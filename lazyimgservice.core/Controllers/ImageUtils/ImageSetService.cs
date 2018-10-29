using lazyimgservice.core.Models;
using System;
using System.IO;


namespace lazyimgservice.core.Controllers
{
    public class ImageSetService
    {

        public static int Build(EFContext db, string fileExt,string fileName,string fullFilePath, string path, int fileId, string name)
        {            
            {
                var group = new ImageGroup();
                group.Name = name;
                group.Updated = DateTime.UtcNow;
                group.Active = true;
                db.ImageGroups.Add(group);
                db.SaveChanges();

                var img = new ImageRecord();
                img.Name = name;
                img.FileId = fileId;
                img.ImageGroupId = group.Id;
                img.ImageSize = ImageSize.src;
                db.Images.Add(img);
                db.SaveChanges();

                // image resizing          
                int w; int h;

                var f2048 = fileName + "_h2048." + fileExt;
                var file2048 = Path.Combine(path, f2048);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file2048, ImageSize.h2048, out w, out h))
                {
                    var fileInfo = new FileInfo(file2048);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f2048, Key = name, FileExt = fileExt, Name =fileName, FileType = FileType.Image, SizeKb = sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h2048;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f1024 = fileName + "_h1024." + fileExt;
                var file1024 = Path.Combine(path, f1024);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file1024, ImageSize.h1024, out w, out h))
                {
                    var fileInfo = new FileInfo(file1024);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f1024, Key = name , FileExt = fileExt, Name = fileName, FileType = FileType.Image ,SizeKb = sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h1024;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f720 = fileName + "_h720." + fileExt;
                var file720 = Path.Combine(path, f720);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file720, ImageSize.h720, out w, out h))
                {
                    var fileInfo = new FileInfo(file720);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f720, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image, SizeKb = sizekb };
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h720;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f480 = fileName + "_h480." + fileExt;
                var file480 = Path.Combine(path, f480);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file480, ImageSize.h480, out w, out h))
                {
                    var fileInfo = new FileInfo(file480);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f480, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image , SizeKb= sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h480;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f240 = fileName + "_h240." + fileExt;
                var file240 = Path.Combine(path, f240);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file240, ImageSize.h240, out w, out h))
                {
                    var fileInfo = new FileInfo(file240);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f240, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image , SizeKb = sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h240;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f128 = fileName + "_h128." + fileExt;
                var file128 = Path.Combine(path, f128);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file128, ImageSize.h128, out w, out h))
                {
                    var fileInfo = new FileInfo(file128);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f128, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image , SizeKb = sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h128;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f64 = fileName + "_h64." + fileExt;
                var file64 = Path.Combine(path, f64);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file64, ImageSize.h64, out w, out h))
                {
                    var fileInfo = new FileInfo(file64);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f64, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image, SizeKb = sizekb };
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h64;
                    db.Images.Add(i);
                    db.SaveChanges();
                }

                var f32 = fileName + "_h32." + fileExt;
                var file32 = Path.Combine(path, f32);
                if (ImageResizeService.ResizeImageFile(fullFilePath, file32, ImageSize.h32, out w, out h))
                {
                    var fileInfo = new FileInfo(file32);
                    var sizekb = fileInfo.Length * 0.001M;
                    var f = new FileRecord() { FilePath = f32, Key = name, FileExt = fileExt, Name = fileName, FileType = FileType.Image ,SizeKb = sizekb};
                    db.Files.Add(f);
                    db.SaveChanges();
                    var i = new ImageRecord() { FileId = f.Id, Name = name, Width = w, Height = h };
                    i.ImageGroupId = group.Id;
                    i.ImageSize = ImageSize.h32;
                    db.Images.Add(i);
                    db.SaveChanges();
                }
                
                db.SaveChanges();

                return group.Id;
            }
        }
    }
}
