using System.IO;
using UnityEngine;

public class PPMWriter
{
  public static void SavePPM(Color[,] pixels, int width, int height, string filePath)
  {
    using StreamWriter writer = new StreamWriter(filePath);

    writer.WriteLine("P3");
    writer.WriteLine($"{width} {height}");
    writer.WriteLine("255");

    for (int j = height - 1; j >= 0; j--)
    {
      for (int i = 0; i < width; i++)
      {
        Color c = pixels[i, j];

        int r = Mathf.Clamp((int)(c.r * 255), 0, 255);
        int g = Mathf.Clamp((int)(c.g * 255), 0, 255);
        int b = Mathf.Clamp((int)(c.b * 255), 0, 255);

        writer.Write($"{r} {g} {b}  ");
      }
      writer.WriteLine();
    }
  }
}
