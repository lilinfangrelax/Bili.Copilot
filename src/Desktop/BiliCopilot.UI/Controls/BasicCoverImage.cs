// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Extensions;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 基础封面图像控件，用于显示和处理图像，支持缩放和模糊效果。
/// </summary>
public sealed partial class BasicCoverImage : ImageExBase
{
    private readonly static BasicScaleEffect _scaleEffect = new();

    /// <summary>
/// 获取图像缓存的子文件夹名称。
/// </summary>
/// <returns>缓存文件夹名称。</returns>
protected override string GetCacheSubFolder()
        => "ImageCache";

    /// <summary>
/// 获取自定义的 HttpClient 实例，用于图像下载。
/// </summary>
/// <returns>HttpClient 实例。</returns>
protected override HttpClient? GetCustomHttpClient()
        => InternalHttpExtensions.ImageClient;

    /// <inheritdoc/>
    /// <summary>
/// 绘制图像，根据宽高比调整目标尺寸并应用模糊效果。
/// </summary>
/// <param name="canvasBitmap">待绘制的 CanvasBitmap 对象。</param>
protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var height = canvasBitmap.Size.Height;
        var aspectRatio = width / height;
        var actualHeight = DecodeWidth / aspectRatio;
        if (Math.Abs(DecodeHeight - actualHeight) > 1)
        {
            DecodeHeight = Math.Round(actualHeight);
            CanvasImageSource = new Microsoft.Graphics.Canvas.UI.Xaml.CanvasImageSource(
                resourceCreator: CanvasDevice.GetSharedDevice(),
                width: (float)DecodeWidth,
                height: (float)DecodeHeight,
                dpi: 96,
                CanvasAlphaMode.Premultiplied);
        }

        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var sourceRect = new Rect(0, 0, width, height);
        DrawBlurImage(canvasBitmap, destRect, sourceRect);
    }

    /// <summary>
/// 绘制模糊图像，根据目标矩形和源矩形调整图像尺寸并应用模糊效果。
/// </summary>
/// <param name="canvasBitmap">待绘制的 CanvasBitmap 对象。</param>
/// <param name="destinationRect">目标矩形区域。</param>
/// <param name="sourceRect">源矩形区域。</param>
private void DrawBlurImage(
        CanvasBitmap canvasBitmap,
        Rect destinationRect,
        Rect sourceRect)
    {
        if (destinationRect.Width <= 0 || destinationRect.Height <= 0)
        {
            return;
        }

        _scaleEffect.Source = canvasBitmap;
        _scaleEffect.SourceRectangle = sourceRect;
        _scaleEffect.DestinationRectangle = destinationRect;
        using var ds = CanvasImageSource!.CreateDrawingSession(Colors.Transparent);
        ds.DrawImage(_scaleEffect, destinationRect, destinationRect);
    }
}
