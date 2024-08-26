using HerosProject.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HerosProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //Upload single product
        [HttpPut("UploadProduct")]
        public async Task<IActionResult> UploadIamge(IFormFile formFile,string productCode)
        {
            APIResponse response = new APIResponse();
            try{
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
                } ;
            }
            catch(Exception ex)
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
            int passcount =0; int Errorcount = 0; 
            try
            {
                string filepath = Getfilepath(productCode);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO .Directory.CreateDirectory(filepath);
                }

                foreach(var file in filecolelntion)
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


        [NonAction]
        private string Getfilepath(string productcode)
        {
            return _webHostEnvironment.WebRootPath + "\\upload\\product\\" + productcode;
        }
    }
}
