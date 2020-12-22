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

        //GET:Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //проверка доступности страницы
                if (dto== null)
                {
                    return Content("The page doesn't exist.");
                }

                //инициализируем модель данными
                model = new PageVM(dto);
            }
            //Возвращаем модель в представление
            return View();
        }

        //POST:Admin/Pages/AddPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //получаем айди страницы
                int id = model.Id;

                //объявляем переменную краткого заголовка
                string slug = "home";

                //получаем страницу (по айди)
                PagesDTO dto = db.Pages.Find(id);

                //присваиваем название из полученной модели в DTO
                dto.Title = model.Title;

                //проверяем краткий заголовок и присваиваем его, если это необходимо
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //проверяем slug and title на уникальность
                if (db.Pages.Where( x=> x.Id != id).Any(x=> x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                //записываем остальные значения в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                //сохраняем изменения в базу
                db.SaveChanges();
            }

            //устанавливаем сообщение в TemData
            TempData["SM"] = "You have edited the page.";
            //переадресация пользователя

            return RedirectToAction("EditPage");

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
