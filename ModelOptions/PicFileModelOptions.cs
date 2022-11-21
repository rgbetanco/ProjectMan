using CSHelper.Extensions.File;
using CSHelper.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace projectman.ModelOptions
{

    class PicFileModelOptions : FileModelOptions<PicFileModel>
    {
        public string OutputThumbnailPath { get; set; }
        public string OutputFullImagePath { get; set; }

        public int MaxWidth { get; set; } = 1024;
        public int MaxHeight { get; set; } = 1024;

        public int MaxThumbnailWidth { get; set; } = 150;
        public int MaxThumbnailHeight { get; set; } = 150;

        public override FileStream ProcessFile(PicFileModel p, string filename_save, ref List<string> tempFiles)
        {
            FileStream fs = base.ProcessFile(p, filename_save, ref tempFiles);

            // prepare an output file path, and save the resized image there
            string filename_resized_temp = GenTempName(p);
            string filename_ext = GetSourceExtension(p);

            string filename_resized = $@"{filename_resized_temp}{filename_ext}";
            string filepath_resized = Path.Join(OutputFullImagePath, filename_resized);

            string filename_resized_thumb = $@"{filename_resized_temp}_thumb{filename_ext}";
            string filepath_resized_thumb = Path.Join(OutputThumbnailPath, filename_resized_thumb);

            using (Image image = Image.Load(fs))
            {
                try
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxWidth, MaxHeight),
                        Sampler = KnownResamplers.Lanczos3
                    }));

                    image.Save(filepath_resized);
                    tempFiles.Add(filepath_resized);

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxThumbnailWidth, MaxThumbnailHeight)
                    }));

                    image.Save(filepath_resized_thumb);
                    tempFiles.Add(filepath_resized_thumb);
                }
                catch (Exception e)
                {
                    throw new ImageProcessUploadError("Unable to generate resized image.", e);
                }
            }

            p.output_file = filename_resized;
            p.thumb_file = filename_resized_thumb;

            return fs;
        }
    }
}
