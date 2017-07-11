﻿using Grand.Core.Configuration;
using ImageSharp;
using SixLabors.Fonts;
using System;

namespace Grand.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        public int AvatarPictureSize { get; set; }
        public int ProductThumbPictureSize { get; set; }
        public int ProductDetailsPictureSize { get; set; }
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }
        public int AssociatedProductPictureSize { get; set; }
        public int CategoryThumbPictureSize { get; set; }
        public int ManufacturerThumbPictureSize { get; set; }
        public int VendorThumbPictureSize { get; set; }
        public int CartThumbPictureSize { get; set; }
        public int MiniCartThumbPictureSize { get; set; }
        public int AutoCompleteSearchThumbPictureSize { get; set; }
        public int ImageSquarePictureSize { get; set; }
        public bool DefaultPictureZoomEnabled { get; set; }

        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Geta or sets a default quality used for image generation
        /// </summary>
        [Obsolete("not supported in ImageSharp")]
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// Geta or sets a vaue indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }

        /// <summary>
        /// Get/set value whether use Pingo Image Compression
        /// </summary>
        public bool UseImageCompress { get; set; }

        #region Watermark Text
        public string WatermarkText { get; set; }
        [Obsolete("not supported in ImageSharp", true)]
        public FontStyle WatermarkStyle { get; set; }
        public Rgba32 WatermarkFontColor { get; set; }
        public Font WatermarkFontFamily { get; set; }
        public int WatermarkPositionXPercent { get; set; }
        public int WatermarkPositionYPercent { get; set; }
        public int WatermarkFontSizePercent { get; set; }
        public int WatermarkOpacityPercent { get; set; }
        [Obsolete("not supported in ImageSharp")]
        public bool WatermarkDropShadow { get; set; }
        [Obsolete("not supported in ImageSharp")]
        public bool WatermarkVertical { get; set; }
        [Obsolete("not supported in ImageSharp")]
        public bool WatermarkRightToLeft { get; set; }

        #endregion

        #region Watermark Misc Options

        public int WatermarkForPicturesAboveSize { get; set; }
        public bool ApplyWatermarkOnPicturesWithOriginalSize { get; set; }
        public bool ApplyWatermarkForProduct { get; set; }
        public bool ApplyWatermarkForCategory { get; set; }
        public bool ApplyWatermarkForManufacturer { get; set; }

        #endregion

        #region Watermark Overlay

        public string WatermarkOverlayID { get; set; }
        public int WatermarkOverlayPositionXPercent { get; set; }
        public int WatermarkOverlayPositionYPercent { get; set; }
        public int WatermarkOverlaySizePercent { get; set; }
        public int WatermarkOverlayOpacityPercent { get; set; }
        public Rgba32 WatermarkOverlayColor { get; set; }

        #endregion
    }
}