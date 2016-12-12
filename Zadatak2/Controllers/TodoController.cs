using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zadatak1;
using Zadatak2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Zadatak2.Models.TodoViewModels;

namespace Zadatak2.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {

        private readonly ITodoRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;



        public TodoController(ITodoRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            List<TodoItem> todoItems;
            try
            {
                todoItems = _repository.GetActive(Guid.Parse(currentUser.Id));
            }
            catch (ArgumentNullException ignorable)
            {
                return View("Error");
            }
            catch (FormatException ignorable)
            {
                return View("Error");
            }
            return View(todoItems);
        }


        public IActionResult Add()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
                TodoItem item;

                try
                {
                    item = new TodoItem(model.Text, Guid.Parse(await _userManager.GetUserIdAsync(currentUser)));
                }
                catch (FormatException ex)
                {
                    // logger
                    return View("Error");
                }
                catch (ArgumentNullException ex)
                {
                    //logger
                    return View("Error");
                }

                try
                {
                    _repository.Add(item);
                    return RedirectToAction("Index");
                }
                catch (DuplicateTodoItemException ex)
                {
                    // logger
                    return View();
                }
                
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Completed()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            List<TodoItem> todoItems;

            try
            {
                todoItems = _repository.GetCompleted(Guid.Parse(currentUser.Id));
            }
            catch (ArgumentNullException ignorable)
            {
                return View("Error");
            }
            catch(FormatException ignorable)
            {
                return View("Error");
            }
            return View(todoItems);
        }

        
        [HttpGet]
        public async Task<IActionResult> MarkAsCompleted(Guid Id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                if (!_repository.MarkAsCompleted(Id,Guid.Parse(currentUser.Id)))
                {
                    return View("Error");
                }
            }
            catch (TodoAccesDeniedException ex)
            {
                // logger
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ignorable)
            {
                return View("Error");
            }
            catch (FormatException ignorable)
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }


    
        public IActionResult Error()
        {
            return View();
        }
    }
}