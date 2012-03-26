#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace nJupiter.Drawing.Imaging {
	/// <summary>
	/// Class that handles scaling of images.
	/// </summary>
	public static class ImageScale {
		#region Enums
		[Flags]
		public enum ResizeFlags {
			None = 0,
			AllowEnlarging = 1,
			AllowStretching = 2
		}
		#endregion

		#region Constructors
		#endregion

		#region Methods
		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Image originalImage, int newWidth, int newHeight) {
			return Resize(originalImage, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Image originalImage, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			MemoryStream ms = new MemoryStream();
			Resize(originalImage, ms, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
			return ms;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		public static void Resize(Image originalImage, Stream outputStream, int newWidth, int newHeight) {
			Resize(originalImage, outputStream, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(Image originalImage, Stream outputStream, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			Resize(originalImage, outputStream, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Image originalImage, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			return Resize(originalImage, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <remarks>Set both newWidth and newHeight to 0 to prevent resizing.</remarks>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Image originalImage, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			MemoryStream ms = new MemoryStream();
			Resize(originalImage, ms, newWidth, newHeight, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
			return ms;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		public static void Resize(Image originalImage, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			Resize(originalImage, outputStream, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="originalImage">The original image to resize</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(Image originalImage, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			if(originalImage == null) {
				throw new ArgumentNullException("originalImage");
			}

			Size originalSize = originalImage.PhysicalDimension.ToSize();
			Size newSize;
			if((resizeFlags & ResizeFlags.AllowStretching) > 0) {
				newSize = GetNonProportionalImageSize(originalSize, newWidth, newHeight, (resizeFlags & ResizeFlags.AllowEnlarging) > 0);
			} else {
				newSize = GetProportionalImageSize(originalSize, newWidth, newHeight, (resizeFlags & ResizeFlags.AllowEnlarging) > 0);
			}

			using(MemoryStream memoryStream = new MemoryStream()) {
				if(newSize.Equals(originalSize)) {
					// Keep original size. Only make a copy
					originalImage.Save(memoryStream, new ImageFormat(originalImage.RawFormat.Guid));
				} else {
					// Create new pic.
					using(Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height)) {
						using(Graphics graphics = Graphics.FromImage(bitmap)) {
							graphics.SmoothingMode = smoothingMode;
							graphics.InterpolationMode = interpolationMode;
							graphics.PixelOffsetMode = pixelOffsetMode;
							graphics.DrawImage(originalImage, 0, 0, bitmap.Width, bitmap.Height);
							bitmap.Save(memoryStream, originalImage.RawFormat);
						}
					}
				}
				memoryStream.WriteTo(outputStream);
			}
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(string imagePath, int newWidth, int newHeight) {
			return Resize(imagePath, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(string imagePath, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			MemoryStream ms = new MemoryStream();
			Resize(imagePath, ms, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
			return ms;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		public static void Resize(string imagePath, Stream outputStream, int newWidth, int newHeight) {
			Resize(imagePath, outputStream, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(string imagePath, Stream outputStream, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			Resize(imagePath, outputStream, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(string imagePath, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			return Resize(imagePath, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default. 
		/// </summary>
		/// <remarks>Set both newWidth and newHeight to 0 to prevent resizing.</remarks>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(string imagePath, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			try {
				MemoryStream ms = new MemoryStream();
				Resize(imagePath, ms, newWidth, newHeight, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
				return ms;
			} catch(FileNotFoundException) {
				// If file not found
			} catch(OutOfMemoryException) {
				// If not image
			}
			return null;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		public static void Resize(string imagePath, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			Resize(imagePath, outputStream, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imagePath">The full path of the image to resize. E.g. 'c:/WebFolder/upload/sample.jpg'</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(string imagePath, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			using(Image origImage = Image.FromFile(imagePath)) {
				Resize(origImage, outputStream, newWidth, newHeight, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
			}
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Stream imageStream, int newWidth, int newHeight) {
			return Resize(imageStream, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Stream imageStream, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			MemoryStream ms = new MemoryStream();
			Resize(imageStream, ms, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
			return ms;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		public static void Resize(Stream imageStream, Stream outputStream, int newWidth, int newHeight) {
			Resize(imageStream, outputStream, newWidth, newHeight, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(Stream imageStream, Stream outputStream, int newWidth, int newHeight, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			Resize(imageStream, outputStream, newWidth, newHeight, ResizeFlags.None, smoothingMode, interpolationMode, pixelOffsetMode);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Stream imageStream, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			return Resize(imageStream, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default. 
		/// </summary>
		/// <remarks>Set both newWidth and newHeight to 0 to prevent resizing.</remarks>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		/// <returns>A memory stream containing the resized image.
		/// Returns null if the original file was neither an image nor found.
		/// </returns>
		public static MemoryStream Resize(Stream imageStream, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			try {
				MemoryStream ms = new MemoryStream();
				Resize(imageStream, ms, newWidth, newHeight, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
				return ms;
			} catch(OutOfMemoryException) {
				// If not image
			}
			return null;
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		public static void Resize(Stream imageStream, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags) {
			Resize(imageStream, outputStream, newWidth, newHeight, resizeFlags, SmoothingMode.Default, InterpolationMode.Default, PixelOffsetMode.Default);
		}

		/// <summary>
		/// Resizes an image to a new size. Resizing neither stretches nor enlarges a picture by default.
		/// </summary>
		/// <param name="imageStream">The stream containing the image to resize.</param>
		/// <param name="outputStream">The stream containing the resized image.</param>
		/// <param name="newWidth">The new width of the image. Set to 0 to let the height decide.</param>
		/// <param name="newHeight">The new height of the image. Set to 0 to let the width decide.</param>
		/// <param name="resizeFlags">Specify the different flags in the ResizeFlags enumeration to deviate from the default behaviour.</param>
		/// <param name="smoothingMode">Specifies the smoothing mode.</param>
		/// <param name="interpolationMode">Specifies the interpolation mode.</param>
		/// <param name="pixelOffsetMode">Specifies the pixel offset mode.</param>
		public static void Resize(Stream imageStream, Stream outputStream, int newWidth, int newHeight, ResizeFlags resizeFlags, SmoothingMode smoothingMode, InterpolationMode interpolationMode, PixelOffsetMode pixelOffsetMode) {
			using(Image origImage = Image.FromStream(imageStream)) {
				Resize(origImage, outputStream, newWidth, newHeight, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
			}
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Calculates a proportional file size.		
		/// </summary>		
		/// <param name="origPhysSize">Original size of image.</param>
		/// <param name="newWidth">Widht requested. Set to 0 to let height decide.</param>
		/// <param name="newHeight">Height requested. Set to 0 to let width decide.</param>
		/// <param name="allowEnlarge">Set to true to allow an image to become larger than original.</param>
		/// <returns>Calculated size.</returns>
		public static Size GetProportionalImageSize(Size origPhysSize, int newWidth, int newHeight, bool allowEnlarge) {
			if(newWidth == 0 && newHeight == 0) {
				return origPhysSize;
			}

			Size newSize;
			float ratio = (float)origPhysSize.Width / origPhysSize.Height;

			if((origPhysSize.Width >= origPhysSize.Height && newHeight != 0 && (newHeight * ratio < newWidth || newWidth == 0)) ||
				(origPhysSize.Width < origPhysSize.Height && !(newWidth != 0 && (newWidth < newHeight * ratio || newHeight == 0)))) {
				newSize = new Size((int)Math.Ceiling(newHeight * ratio), newHeight);
			} else {
				newSize = new Size(newWidth, (int)Math.Ceiling(newWidth / ratio));
			}

			if(((newSize.Height > origPhysSize.Height || newSize.Width > origPhysSize.Width) && allowEnlarge) ||
				newSize.Width < origPhysSize.Width || newSize.Height < origPhysSize.Height) {
				return newSize;
			}
			return origPhysSize;
		}
		public static Size GetNonProportionalImageSize(Size origPhysSize, int newWidth, int newHeight, bool allowEnlarge) {
			return (newWidth > origPhysSize.Width || newHeight > origPhysSize.Height) && !allowEnlarge ?
			origPhysSize : new Size(newWidth == 0 ? origPhysSize.Width : newWidth, newHeight == 0 ? origPhysSize.Height : newHeight);
		}
		#endregion
	}
}