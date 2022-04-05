﻿using System.Drawing;
using static System.Console;
using static System.Drawing.Image;

int strangeShakal = 1;

WriteLine("Where ukraine flags?");
Bitmap startImage = ((Bitmap)FromFile("C:\\ukraine.jpg")).CutSize(strangeShakal);

int six = startImage.Width, siy = startImage.Height,
ix = startImage.Width / strangeShakal, iy = startImage.Height / strangeShakal;

Bitmap image = new(ix, iy);
Vec3[,] imageColors = new Vec3[ix, iy];

ImageShakaling();

for (int x = 0; x < image.Width; x++)
    for (int y = 0; y < image.Height; y++)
        image.SetPixel(x, y, imageColors[x, y].ToColor());

List<(Point, Vec3)> flagFind = new List<(Point, Vec3)>();

for (int x = 0; x < ix; x++)
    for (int y = 0; y < iy; y++)
        if (imageColors[x, y].IsColor(200, ColorType.B) || imageColors[x, y].IsColor(150, ColorType.R, ColorType.G))
            flagFind.Add((new(x, y), imageColors[x, y]));

List<int> coordsX = flagFind.Select(z => z.Item1.X).Distinct().ToList();
List<(int x, List<(Point, Vec3)> yellows, List<(Point, Vec3)> blues, int startY, int yellowCount, int blueCount, int allCount, int perColor, int extraLastColor)> rawPixels = new List<(int x, List<(Point, Vec3)> yellows, List<(Point, Vec3)> blues, int startY, int yellowCount, int blueCount, int allCount, int perColor, int extraLastColor)>();
foreach(int x in coordsX)
{
    List<(Point, Vec3)> yellows = flagFind.FindAll(z => z.Item1.X == x && z.Item2.IsColor(150, ColorType.R, ColorType.G));
    List<(Point, Vec3)> blues = flagFind.FindAll(z => z.Item1.X == x && z.Item2.IsColor(200, ColorType.B));
    int startY = flagFind.FindAll(z => z.Item1.X == x && (z.Item2.IsColor(150, ColorType.R, ColorType.G) || z.Item2.IsColor(200, ColorType.B))).OrderBy(z => z.Item1.Y).First().Item1.Y;
    int yellowCount = yellows.Count;
    int blueCount = blues.Count;
    int allCount = yellowCount + blueCount;
    int perColor = allCount / 3;
    int extraLastColor = allCount % 3;
    rawPixels.Add((x, yellows, blues, startY, yellowCount, blueCount, allCount, perColor, extraLastColor));
}
List<(int, int)> rawOneOfThreeList = rawPixels.Where(z => z.yellowCount > 0 && z.blueCount > 0).Select(z => (z.yellowCount, z.blueCount)).ToList();
int maxOneOfThree = rawOneOfThreeList.GroupBy(z => z.Item2).ToDictionary(z => z.Key, g => g.Count()).ToList().Select(z => z.Value).Max();
foreach((int x, List<(Point, Vec3)> yellows, List<(Point, Vec3)> blues, int startY, int yellowCount, int blueCount, int allCount, int perColor, int extraLastColor) pixel in rawPixels)
{
    if((pixel.blueCount + pixel.yellowCount) / 3 > maxOneOfThree)
    {
        for (int w = 0; w < pixel.perColor; w++)
            image.SetPixel(pixel.x, pixel.startY + w, Color.White);
        for (int b = pixel.perColor; b < pixel.perColor * 2; b++)
            image.SetPixel(pixel.x, pixel.startY + b, Color.Blue);
        for (int r = pixel.perColor * 2; r < pixel.perColor * 3; r++)
            image.SetPixel(pixel.x, pixel.startY + r, Color.Red);
        for (int e = 0; e < pixel.extraLastColor + 1; e++)
            image.SetPixel(pixel.x, pixel.startY + pixel.perColor * 3 + e, Color.Red);
    }
    else
    {

        //int count = rawPixels.Find(z => z.x == pixel.x).yellowCount + rawPixels.Find(z => z.x == pixel.x).blueCount;
        for (int w = 0; w < pixel.perColor; w++)
            if (flagFind.FindIndex(z => z.Item1.X == pixel.x && z.Item1.Y == pixel.startY + w) != -1)                
                image.SetPixel(pixel.x, pixel.startY + w, Color.White);
        for (int b = pixel.perColor; b < pixel.perColor * 2; b++)
            if (flagFind.FindIndex(z => z.Item1.X == pixel.x && z.Item1.Y == pixel.startY + b) != -1)
                image.SetPixel(pixel.x, pixel.startY + b, Color.Blue);
        for (int r = pixel.perColor * 2; r < pixel.perColor * 3; r++)
            if (flagFind.FindIndex(z => z.Item1.X == pixel.x && z.Item1.Y == pixel.startY + r) != -1)
                image.SetPixel(pixel.x, pixel.startY + r, Color.Red);
        /*
        for (int w = 0; w < (count - maxOneOfThree < 0 ? maxOneOfThree : count - maxOneOfThree); w++)
            image.SetPixel(pixel.x, pixel.startY + w, Color.White);
        for (int b = (count - maxOneOfThree < 0 ? maxOneOfThree : count - maxOneOfThree); b < (count - maxOneOfThree * 2 < 0 ? maxOneOfThree : count - maxOneOfThree * 2); b++)
            image.SetPixel(pixel.x, pixel.startY + b, Color.Blue);
        for (int r = count - maxOneOfThree * 2; r < (count - maxOneOfThree * 3 < 0 ? maxOneOfThree : count - maxOneOfThree * 3); r++)
            image.SetPixel(pixel.x, pixel.startY + r, Color.Red);/*
        for (int e = pixel.perColor * 3; e < pixel.extraLastColor; e++)
            image.SetPixel(pixel.x, pixel.startY + e, Color.Red);
        */
    }
    WriteLine(pixel.yellowCount + " - " + pixel.blueCount);
}

image.Save("C:\\ucraine.png");

void ImageShakaling()
{
    for (int x = 0; x < ix; x++)
        for (int y = 0; y < iy; y++)
        {
            Vec3[] colors = new Vec3[(int)Math.Pow(strangeShakal, 2)];
            for (int cx = 0; cx < strangeShakal; cx++)
                for (int cy = 0; cy < strangeShakal; cy++)
                    colors[cx * strangeShakal + cy] = startImage.GetPixel(x * strangeShakal + cx, y * strangeShakal + cy).ToVec3();
            Vec3 result = colors[0];
            colors.Skip(1).ToList().ForEach(z => result = result | z);
            imageColors[x, y] = result;
        }
}
struct Range
{
    public Range(int start, int end)
    {
        Start = start;
        End = end;
    }
    public int Start { get; set; }
    public int End { get; set; }
}
struct Vec3
{
    public Vec3(float r, float g, float b, int step)
    {
        R = r;
        G = g;
        B = b;
        Step = step;
    }
    public Vec3(float r, float g, float b) : this(r, g, b, 1) { }
    public int Step { get; set; }
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public static Vec3 operator |(Vec3 v1, Vec3 v2) => new((v1.R * v1.Step + v2.R * v2.Step) / (v1.Step + v2.Step), (v1.G * v1.Step + v2.G * v2.Step) / (v1.Step + v2.Step), (v1.B * v1.Step + v2.B * v2.Step) / (v1.Step + v2.Step), v1.Step + v2.Step);
    public static Vec3 operator -(Vec3 v1, Vec3 v2) {
        Vec3 result = new(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
        return result;
    }
}
static class Static 
{
    public static bool InRange(this Range range, int value) => range.Start <= value && range.End >= value;
    public static Vec3 ToVec3(this Color color) => new(color.R, color.G, color.B);
    public static Vec3 Abs(this Vec3 color) => new(Math.Abs(color.R), Math.Abs(color.G), Math.Abs(color.B), color.Step);
    public static float Sum(this Vec3 color) => color.R + color.G + color.B; 
    public static Color ToColor(this Vec3 color) => Color.FromArgb((int)color.R, (int)color.G, (int)color.B);
    public static Bitmap CutSize(this Bitmap bitmap, int size)
    {
        Bitmap newBitmap = new Bitmap(bitmap.Width - bitmap.Width % size, bitmap.Height - bitmap.Height % size);
        for (int x = 0; x < newBitmap.Width; x++) 
            for (int y = 0; y < newBitmap.Height; y++)
                newBitmap.SetPixel(x, y, bitmap.GetPixel(x, y));
        return newBitmap;
    }
    public static bool IsColor(this Vec3 color, int extra, params ColorType[] type)
    {
        Vec3 perfectColor = Color.FromArgb(type.Contains(ColorType.R) ? 255 : 0, type.Contains(ColorType.G) ? 255 : 0, type.Contains(ColorType.B) ? 255 : 0).ToVec3();
        Vec3 wrongColor = (color - perfectColor).Abs();
        int wrong = (int)wrongColor.Sum();
        return wrong < extra; 
    }
}
enum ColorType { R, G, B }