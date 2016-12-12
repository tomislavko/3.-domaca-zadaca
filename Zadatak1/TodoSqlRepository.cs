using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Zadatak1
{
    public class TodoSqlRepository : ITodoRepository
    {

        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }



        public void Add(TodoItem todoItem)
        { 
            using (_context)
            {
                if (todoItem == null)
                {
                    throw new ArgumentNullException();
                }
                if (_context.TodoItems.Any(t => t.Id == todoItem.Id))
                {
                    throw new DuplicateTodoItemException("Duplicate id: " + todoItem.Id);
                }
                _context.TodoItems.Add(todoItem);
                _context.SaveChanges();
            }
        }


        public TodoItem Get(Guid todoId, Guid userId)
        {
            using (_context)
            {
                return InternalGet(todoId, userId);
            }
        }

        public List<TodoItem> GetActive(Guid userId)
        {
            using (_context)
            {
                return _context.TodoItems.Where(t => !t.IsCompleted).ToList();

            }
        }

        public List<TodoItem> GetAll(Guid userId)
        {
            using (_context)
            {
                return _context.TodoItems.OrderByDescending(t => t.DateCreated).ToList();

            }
        }

        public List<TodoItem> GetCompleted(Guid userId)
        {
            using (_context)
            {
                return _context.TodoItems.Where(t => t.IsCompleted).ToList();

            }
        }

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            using (_context)
            {
                return _context.TodoItems.Where(t => t.UserId == userId).Where(t => filterFunction(t)).ToList();

            }
        }

        public bool MarkAsCompleted(Guid todoId, Guid userId)
        {
            using (_context)
            {
                // throws a TodoAccesDeniedException
                TodoItem item = InternalGet(todoId, userId);

                if (item == null)
                {
                    return false;
                }
                _context.TodoItems.Remove(item);
                _context.SaveChanges();

                item.IsCompleted = true;
                item.DateCompleted = DateTime.Now;
                _context.TodoItems.Add(item);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Remove(Guid todoId, Guid userId)
        {
            using (_context)
            {
                // throws a TodoAccesDeniedException
                TodoItem item = InternalGet(todoId, userId);
                if (item == null)
                {
                    return false;
                }
                _context.TodoItems.Remove(item);
                _context.SaveChanges();
                return true;
            }
        }

        public void Update(TodoItem todoItem, Guid userId)
        {
            using (_context)
            {
                // throws a TodoAccesDeniedException
                TodoItem item = InternalGet(todoItem.Id, userId);
                if (item != null)
                {
                    _context.TodoItems.Remove(item);
                    _context.SaveChanges();

                    item.DateCompleted = todoItem.DateCompleted;
                    item.DateCreated = todoItem.DateCreated;
                    item.IsCompleted = todoItem.IsCompleted;
                    item.Text = todoItem.Text;
                    item.UserId = userId;
                }
                _context.TodoItems.Add(item);
                _context.SaveChanges();
            }
        }

        private TodoItem InternalGet (Guid todoId, Guid userId)
        {
            List<TodoItem> list = _context.TodoItems.Where(t => t.Id == todoId).ToList();
            if (list == null || list.Count() == 0)
            {
                return null;
            }
            if (list.Select(t => t.UserId == userId).Count() == 0)
            {
                throw new TodoAccesDeniedException("User " + userId + " is not owner of the TodoItem!");
            }
            return _context.TodoItems.FirstOrDefault(t => t.Id == todoId);
        }
    }

    public class DuplicateTodoItemException : Exception
    {
        public DuplicateTodoItemException()
        {
        }

        public DuplicateTodoItemException(string message) : base(message)
        {
        }

        public DuplicateTodoItemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateTodoItemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class TodoDbContext : DbContext
    {

        public TodoDbContext(string conectionString) : base(conectionString)
        {

        }

        public IDbSet<TodoItem> TodoItems { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>().HasKey(i => i.Id);
            modelBuilder.Entity<TodoItem>().Property(i => i.Text).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(i => i.IsCompleted).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(i => i.DateCompleted).IsOptional();
            modelBuilder.Entity<TodoItem>().Property(i => i.DateCreated).IsRequired();
            modelBuilder.Entity<TodoItem>().Property(i => i.UserId).IsRequired();
        }
    }

    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCompleted { set; get; }

        /// <summary>
        /// Nullable date time.
        /// DateTime is value type and won't allow nulls.
        /// DateTime? is nullable DateTime and will accepti nulls.
        /// Use null when todo completed date does not exist (e.g. todo is still not completed)
        /// </summary>
        public DateTime? DateCompleted { get; set; }
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// User id that owns this TodoItem
        /// </summary>
        public Guid UserId { get; set; }

        public TodoItem(string text, Guid userId)
        {
            Id = Guid.NewGuid();
            Text = text;
            IsCompleted = false;
            DateCreated = DateTime.Now;
            UserId = userId;
        }

        public TodoItem()
        {

        }

    }

    public class TodoAccesDeniedException : Exception
    {
        public TodoAccesDeniedException()
        {
        }

        public TodoAccesDeniedException(string message) : base(message)
        {
        }

        public TodoAccesDeniedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TodoAccesDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
