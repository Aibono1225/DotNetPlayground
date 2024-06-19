using ArchPractice.Common;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System.Reflection;

namespace ArchPractice.Extension
{
    /// <summary>
    /// 拦截器AOP，继承IInterceptor接口
    /// </summary>
    public class ServiceAOP : IInterceptor
    {
        /// <summary>
        /// 实例化IInterceptor的唯一方法
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            string json;
            try
            {
                json = JsonConvert.SerializeObject(invocation.Arguments);
            }
            catch (Exception ex)
            {
                json = "无法序列化，可能是Lambda表达式等原因造成，错误信息：" + ex.ToString();
            }
            
            DateTime startTime = DateTime.Now;
            AOPLogInfo apiLogAopInfo = new AOPLogInfo
            {
                RequestTime = startTime.ToString("yyyy--MM-dd hh:mm:ss fff"),
                OpUserName = "",
                RequestMethodName = invocation.Method.Name,
                RequestParamsName = string.Join(", ", invocation.Arguments.Select(
                    a => (a ?? "").ToString()).ToArray()),
                ResponseJsonData = json
            };

            try
            {
                invocation.Proceed();


                if (IsAsyncMethod(invocation.Method))
                {
                    // 等待任务执行并修改返回值
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                            (Task)invocation.ReturnValue,
                            async () => await SuccessAction(invocation, apiLogAopInfo, startTime),
                            ex => { 
                                LogEx(ex, apiLogAopInfo);
                            });
                    }
                    //Task<TResult>
                    else
                    {
                        invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                            invocation.Method.ReturnType.GenericTypeArguments[0],
                            invocation.ReturnValue,
                            async (o) => await SuccessAction(invocation, apiLogAopInfo, startTime, o),
                            ex =>
                            {
                                LogEx(ex, apiLogAopInfo);
                            });
                    }
                }
                else
                {
                    // 同步
                    string jsonResult;
                    try
                    {
                        jsonResult = JsonConvert.SerializeObject(invocation.ReturnValue);
                    }
                    catch (Exception ex)
                    {
                        jsonResult = "无法序列化，可能是Lambda表达式等原因造成，错误信息：" + ex.ToString();
                    }

                    DateTime endTime = DateTime.Now;
                    string ResponseTime = (endTime - startTime).Milliseconds.ToString();
                    apiLogAopInfo.ResponseTime = endTime.ToString("yyyy-MM-dd hh:mm:ss fff");
                    apiLogAopInfo.ResponseIntervalTime = ResponseTime + "ms";
                    apiLogAopInfo.ResponseJsonData = jsonResult;

                    Console.WriteLine(JsonConvert.SerializeObject(apiLogAopInfo));
                }
            }
            catch (Exception ex)
            {
                LogEx(ex, apiLogAopInfo);
                throw;
            }
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            //检查返回类型是否是 Task 或者 Task<T>
            return method.ReturnType == typeof(Task) ||
                method.ReturnType.IsGenericType &&
                method.ReturnType.GetGenericTypeDefinition() == typeof(Task);
        }

        private async Task SuccessAction(IInvocation invocation, AOPLogInfo apiLogAopInfo, DateTime startTime, object o = null)
        {
            DateTime endTime = DateTime.Now;
            string ResponseTime = (endTime - startTime).ToString();
            apiLogAopInfo.ResponseTime = endTime.ToString("yyyy-MM-dd hh:mm:ss fff");
            apiLogAopInfo.ResponseIntervalTime = ResponseTime + "ms";
            apiLogAopInfo.ResponseJsonData = JsonConvert.SerializeObject(o);

            await Task.Run(() =>
            {
                Console.WriteLine("执行成功-->" + JsonConvert.SerializeObject(apiLogAopInfo));
            });
        }

        private void LogEx(Exception ex, AOPLogInfo dataIntercept)
        {
            if (ex != null)
            {
                Console.WriteLine("Error:" + ex.Message + JsonConvert.SerializeObject(dataIntercept));
            }
        }
    }


    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<object, Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                await postAction(result);
                return result;
            }
            catch (Exception ex)
            {
                exception= ex;
                throw;
            }
            finally {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 使用反射调用泛型方法
        /// </summary>
        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<object, Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }
}
