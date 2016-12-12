using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Zadatak2.Models.TodoViewModels
{
    public class AddTodoViewModel
    {
        [Required]
        [Display(Name = "Todo name")]
        public string Text { get; set; }


    }
}
