using System;
using System.IO;
using System.Threading.Tasks;
using MarkDownAvalonia.Data;
using Microsoft.AspNetCore.Mvc;

namespace MarkDownAvalonia.Controls.Command
{
    [Controller]
    public class IndexController : Controller
    {
        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // 指定HTML文件的路径，确保路径是正确的  
            string filePath = Path.Combine(CommonData.config.PostDirectory, "preview.html");  
  
            Console.WriteLine("index path" + filePath);
            // 检查文件是否存在  
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }  
  
            // 读取文件内容  
            string fileContent = await System.IO.File.ReadAllTextAsync(filePath);  
  
            // 返回文件内容作为text/html  
            return Content(fileContent, "text/html");  
        }
    }
}