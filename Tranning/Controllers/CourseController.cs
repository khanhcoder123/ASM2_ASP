using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class CourseController : Controller
    {
        private readonly TranningDBContext _dbContext;
        private readonly ILogger<CourseController> _logger;

        public CourseController(TranningDBContext context, ILogger<CourseController> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            CourseModel course = new CourseModel();
            course.CourseDetailLists = _dbContext.Courses
                .Select(item => new CourseDetail
                {
                    category_id = item.category_id,
                    id = item.id,
                    name = item.name,
                    description = item.description,
                    avatar = item.avatar,
                    status = item.status,
                    start_date = item.start_date,
                    end_date = item.end_date,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                }).ToList();

            ViewData["CurrentFilter"] = "";
            return View(course);
        }

        [HttpGet]
        public IActionResult Add()
        {
            CourseDetail course = new CourseDetail();
            PopulateCategoryDropdown();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CourseDetail course, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = await UploadFile(Photo);
                    var courseData = new Course()
                    {
                        category_id = course.category_id,
                        name = course.name,
                        description = course.description,
                        avatar = uniqueFileName,
                        status = course.status,
                        start_date = course.start_date,
                        end_date = course.end_date,
                        created_at = DateTime.Now
                    };

                    _dbContext.Courses.Add(courseData);
                    _dbContext.SaveChanges();

                    TempData["saveStatus"] = true;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while adding a course.");
                    TempData["saveStatus"] = false;
                }
            }

            // If ModelState is not valid, return to the Add view with validation errors
            PopulateCategoryDropdown();
            return View(course);
        }

        private async Task<string> UploadFile(IFormFile file)
        {
            string uniqueFileName;
            try
            {
                string pathUploadServer = "wwwroot\\uploads\\images";

                string fileName = Path.GetFileName(file.FileName);
                string uniqueStr = Guid.NewGuid().ToString();
                fileName = uniqueStr + "-" + fileName;
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathUploadServer, fileName);

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uniqueFileName = fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading a file.");
                uniqueFileName = ex.Message.ToString();
            }
            return uniqueFileName;
        }

        private void PopulateCategoryDropdown()
        {
            ViewBag.Stores = _dbContext.Categories
                .Where(m => m.deleted_at == null)
                .Select(m => new SelectListItem { Value = m.id.ToString(), Text = m.name })
                .ToList();
        }
    }
}
