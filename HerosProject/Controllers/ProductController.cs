using HerosProject.Data;
using HerosProject.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace HerosProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        public ProductController(IWebHostEnvironment webHostEnvironment,DataContext dataContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dataContext = dataContext;
        }
        //Upload single product
        [HttpPut("UploadProduct")]
        public async Task<IActionResult> UploadIamge(IFormFile formFile, string productCode)
        {
            APIResponse response = new APIResponse();
            try {
                string filePath = Getfilepath(productCode);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                string imagepath = filePath + "\\" + productCode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.statucode = "200";
                    response.Result = "successed";
                };
            }
            catch (Exception ex)
            {
                response.ErroMessage = ex.Message;
            }
            return Ok(response);
        }

        //Upload multiple product
        [HttpPut("UploadMultipple/{productCode}")]
        public async Task<IActionResult> UploadMultiple(IFormFileCollection filecolelntion, string productCode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int Errorcount = 0;
            try
            {
                string filepath = Getfilepath(productCode);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }

                foreach (var file in filecolelntion)
                {
                    string imagepath = filepath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;
                    };
                }
            }
            catch (Exception ex)
            {
                Errorcount++;
                response.ErroMessage = ex.Message;
            }
            response.statucode = "200";
            response.Result = "Passcount " + passcount + " ErrorCount " + Errorcount;
            return Ok(response);
        }

        [HttpGet("GetImaege/{productCode}")]
        public async Task<IActionResult> GetImage(string productCode)
        {
            string ImageURL = string.Empty;
            string HostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string FilePath = Getfilepath(productCode);
                string ImagePath = FilePath + "\\" + productCode + ".png";
                if (System.IO.File.Exists(ImagePath))
                {
                    ImageURL = HostUrl + "\\upload\\product\\" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound(ImageURL);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(ImageURL);
        }

        [HttpGet("SetOfProduct/{productCode}")]
        public async Task<IActionResult> ReturnSetOfImages(string productCode)
        {
            List<string> images = new List<string>();
            string HostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string FilePath = Getfilepath(productCode);
                if (System.IO.Directory.Exists(FilePath))
                {
                    DirectoryInfo DirectoryInfo = new DirectoryInfo(FilePath);
                    FileInfo[] fileInfos = DirectoryInfo.GetFiles();
                    foreach (FileInfo file in fileInfos)
                    {
                        string fileName = file.Name;
                        string filepath = FilePath + "\\" + fileName;
                        if (System.IO.File.Exists(filepath))
                        {
                            string ImageURL = HostUrl + "\\upload\\product\\" + productCode + "/" + fileName;
                            images.Add(ImageURL);
                        }
                    }
                }
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);

            }
            return Ok(images);
        }

        [HttpGet("Download/{procuctCode}")]
        public async Task<IActionResult> DownloasLink(string procuctCode)
        {
            try
            {
                string Filepath = Getfilepath(procuctCode);
                string Imagepath = Filepath + "\\" + procuctCode + ".png";
                if (System.IO.File.Exists(Imagepath))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(Imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(memoryStream);
                    }
                    memoryStream.Position = 0;
                    return File(memoryStream, "image/png", procuctCode + ".png");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("test");
        }

        [HttpDelete]
        [Route("RemoveMultiple")]
        public async Task<IActionResult> DeletePicture(string procuctCode)
        {
            try
            {
                string Filepath = Getfilepath(procuctCode);
                string Imagepath = Filepath + "\\" + procuctCode + ".png";
                DirectoryInfo DirectoryInfo = new DirectoryInfo(Filepath);
                FileInfo[] fileInfos = DirectoryInfo.GetFiles();
                foreach (FileInfo file in fileInfos)
                {
                    string fileName = file.Name;
                    string filepath = Filepath + "\\" + fileName;
                    if (System.IO.File.Exists(filepath))
                    {
                        System.IO.File.Delete(filepath);
                    }
                }
                Ok("Deletes Successfully");

            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok();
        }

        //Upload multiple product
        [HttpPut("DBUploadMultipple/{productCode}")]
        public async Task<IActionResult> DBUploadMultiple(IFormFileCollection filecolelntion, string productCode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int Errorcount = 0;
            try
            {
                foreach (var file in filecolelntion)
                {
                  
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        _dataContext.productImages.AddAsync(new ProductImage
                        {
                            ProductCode = productCode,
                            productName = stream.ToArray()
                        });
                        passcount++;
                        _dataContext.SaveChangesAsync();
                    };
                }
            }
            catch (Exception ex)
            {
                Errorcount++;
                response.ErroMessage = ex.Message;
            }
            response.statucode = "200";
            response.Result = "Passcount " + passcount + " ErrorCount " + Errorcount;
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMultiple(string procuctCode)
        {
            try
            {
                string Filepath = Getfilepath(procuctCode);
                string Imagepath = Filepath + "\\" + procuctCode + ".png";
                if (System.IO.File.Exists(Imagepath))
                {
                    System.IO.File.Delete(Imagepath);

                    return Ok(Imagepath + "Delete successfully");
                }

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok();
        }

        //Upload single product
        [HttpPut("DBUploadProduct")]
        public async Task<IActionResult> DBUploadIamge(IFormFile formFile, string productCode)
        {
            APIResponse response = new APIResponse();
            try
            {
                using ( MemoryStream stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    _dataContext.productImages.AddAsync(new ProductImage
                    {
                        ProductCode = productCode,
                        productName = stream.ToArray()
                    });
                     await _dataContext.SaveChangesAsync();
                    response.statucode = "200";
                    response.Result = "successed";
                };
            }
            catch (Exception ex)
            {
                response.ErroMessage = ex.Message;
            }
            return Ok(response);
        }

        //Download from database
        [HttpGet("GetImageDownload")]
        public async Task<IActionResult> DBdownload(string productCode)
        {
            List<string> Image  = new List<string>();
            try
            {
                var result = _dataContext.productImages.Where(x => x.ProductCode==productCode).ToList();
                if(result != null)
                {
                    foreach (var item in result)
                    {
                        Image.Add(Convert.ToBase64String(item.productName));
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok(Image);
        }

        [HttpGet("DBDownloadFile")]
        public async Task<IActionResult> DbDownloadFile(string productCode)
        {
            try
            {
                var result = _dataContext.productImages.FirstOrDefault(x => x.ProductCode == productCode);
                if (result != null)
                {
                    return File(result.productName,"image/png", productCode);
                }

            }
            catch (Exception ex) { 
            
                return NotFound(ex.Message);
            }

            return Ok();

        }




        [NonAction]
        private string Getfilepath(string productcode)
        {
            return _webHostEnvironment.WebRootPath + "\\upload\\product\\" + productcode;
        }
    }
}
