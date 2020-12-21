using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Store.Models.Data;

namespace Store.Models.ViewModels.Pages
{
    public class PageVM
    {   
        public PageVM()//конструктор класса по умолчанию, если вдруг у нас не будет открываться наш конструтор
        {

        }
 
        
        public  PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50,MinimumLength = 3)]
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength =3)]
        public int Sorting { get; set; }
        [Display(Name ="Sidebar")]
        public bool HasSidebar { get; set; }
    }
}