using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Store.Models.Data;
using Store.Models.ViewModels.Pages;

namespace Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //объявляем список для представления (PageVM)
            List<PageVM> pageList;

            //инициализируем список (DB)
            using (Db db = new Db())
            {
                //pageList'у присваиваем объекты из базы данных, но сперва вносим все в массив, сортируем,
                //после добавляем отсортированные данные в лист
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //возвращаем список в представление
            return View(pageList);
        }

        //GET:Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        //POST:Admin/Pages/AddPage
        public ActionResult AddPage(PageVM model){
        
            //проверка введеных данных от пользователя
            if (!ModelState.IsValid){
            
                return View(model);
            }

            using (Db db = new Db()){
            
                //переменная для краткого описания (slug)
                string slug;
                //инициализация класса PageDTO для работы с базой данных
                PagesDTO dto = new PagesDTO{
                
                    //заголовок нашей модели
                    Title = model.Title.ToUpper()
                };
                //проверка краткого описание, если нет то присваиваем его
                if (string.IsNullOrWhiteSpace(model.Slug)){
                
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else{
                
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //проверка на оригинальность заголовка и краткого описания 
                if (db.Pages.Any(x => x.Title == model.Title)) {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                //присваиваем оставшиеся значения модели 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //сохраняем в базу данных нашу модель 
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //передать сбщ пользователю успешно или не успешна операция внесения новой модели TempData
            TempData["SM"] = "You have added a new page";
            //переадресация пользователя на метод INDEX
            return RedirectToAction("Index");
        }

        //GET:Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //объявляем модель PageVM
            PageVM model;
            using (Db db = new Db())
            {
                //получаем страницу 
                PagesDTO dto = db.Pages.Find(id);
                //Подтверждаем что страница доступна 
                if (dto == null)
                {
                    return Content("The page doesn't exist.");
                }
                // ПРисваиваем модели информацию из базы
                model = new PageVM(dto);
            
            }
                //Возвращаем модель в представление
                return View();
        }
    }
 }
