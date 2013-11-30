using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoProcess.PhotoEdit
{
    public class EditImage
    {
        public static  WriteableBitmap ToGrayBitmap(WriteableBitmap originalBitmap)
        {            
            int[] orgImageData = originalBitmap.Pixels;
            
            int heihgt = originalBitmap.PixelHeight;
            int width = originalBitmap.PixelWidth;
            WriteableBitmap processedBitmap = new WriteableBitmap(width, heihgt);
            int[] newImageData = processedBitmap.Pixels;

            for( int j = 0; j < heihgt; j ++ )
                for( int i = 0; i< width; i++)
                {
                    int pos = j * width + i;
                    int color = orgImageData[pos];
                    byte r = (byte)(color >> 16); // R
                    byte g = (byte)(color >> 8);  // G
                    byte b = (byte)(color);       // B
                    byte c = (byte)( (r + g + b) / 3 );

                    newImageData[pos] = (0xFF << 24)                    // A
                         | (c << 16) // R
                         | (c << 8)  // G
                         | c;  // B
                }
            return processedBitmap;
        }

        public static WriteableBitmap RotateBitmap(WriteableBitmap originalBitmap)
        {
            int orgWidth =  originalBitmap.PixelWidth;
            int orgHeight = originalBitmap.PixelHeight;
            int newWidth = orgHeight;
            int newHeight = orgWidth;
            WriteableBitmap processedBitmap = new WriteableBitmap(newWidth, newHeight);
            int[] orgImageData = originalBitmap.Pixels;
            int[] newImageData = processedBitmap.Pixels;


            for (int j = 0; j < newHeight; j++)
                for (int i = 0; i < newWidth; i++)
                {
                    int newPos = j * newWidth + i;
                    int color = mapToOriginalImage(orgImageData, orgWidth, orgHeight, i, j);
                    byte r = (byte)(color >> 16); // R
                    byte g = (byte)(color >> 8);  // G
                    byte b = (byte)(color);       // B                  

                    newImageData[newPos] = (0xFF << 24)                    // A
                         | (r << 16) // R
                         | (g << 8)  // G
                         | b;  // B
                }
            return processedBitmap;
        }

        private static int mapToOriginalImage(int[] orgImageData, int orgWidth, int orgHeight, int newCol, int newRow )
        {
            int orgRow = orgHeight - 1 - newCol;
            int orgCol = newRow;
            int orgPos = orgRow * orgWidth + orgCol;
            return orgImageData[orgPos]; 
        }

    }
}
