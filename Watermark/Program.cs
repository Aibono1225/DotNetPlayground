using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

Console.WriteLine("Hello, World!");


const string text = "watermark text";
const string filePath = "C:\\Users\\ChloeLin\\Pictures";

const float WatermarkPadding = 2f;
const string WatermarkFont = "Roboto";
const float WatermarkFontSize = 34f;

var image = await Image.LoadAsync($"{filePath}\\panda.png");

//using (var image = await Image.LoadAsync("C:\\Users\\ChloeLin\\Pictures\\panda.png"))
//{
//    image.Save("output.png");
//}


async Task Resize(Image input, int width, int height)
{
    var resizedImg = input.Clone(x => x.Resize(width, height));
    await resizedImg.SaveAsync($"{filePath}\\output.png");
}


async Task AddWaterMark(Image input)
{

    FontFamily fontFamily;

    if (!SystemFonts.TryGet(WatermarkFont, out fontFamily))
    {
        throw new Exception($"Couldn't find out {WatermarkFont}");
    }

    var font = fontFamily.CreateFont(WatermarkFontSize, FontStyle.Regular);

    var options = new TextOptions(font)
    {
        Dpi = 72,
        KerningMode = KerningMode.Standard
    };

    var rect = TextMeasurer.MeasureSize(text, options);

    input.Mutate(x => x.DrawText(
        text,
        font,
        //new Color(Rgba32.ParseHex("#FFF")),
        Color.Orange,
        new PointF(input.Width - rect.Width - WatermarkPadding,
            input.Height - rect.Height - WatermarkPadding))
    );

    await input.SaveAsPngAsync($"{filePath}\\watermark.png");
    //await input.SaveAsJpegAsync($"{filePath}\\watermark.jpg");
}


//await Resize(image, 50, 50);
//await AddWaterMark(image);








