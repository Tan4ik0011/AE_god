using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WinFormsEFCoreApp;
using static WinFormsEFCoreApp.Program;

namespace WinFormsEFCoreApp.Controllers
{
    public class MethodController
    {
        private readonly AppDbContext _context;

        public MethodController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все методы
        public IQueryable<Method> GetMethods()
        {
            return _context.Methods
                .Include(m => m.Class)  // Включаем связанный класс для каждого метода
                .AsQueryable();
        }

        // Получить метод по ID
        public Method GetMethodById(int id)
        {
            return _context.Methods
                .Include(m => m.Class)
                .FirstOrDefault(m => m.MethodId == id);
        }

        // Добавить новый метод
        public void AddMethod(string methodName, int classId, int startLine, int endLine)
        {
            var methodEntity = new Method
            {
                MethodName = methodName,
                ClassId = classId,
                StartLine = startLine,
                EndLine = endLine
            };

            _context.Methods.Add(methodEntity);
            _context.SaveChanges();
        }

        // Удалить метод по ID
        public void DeleteMethod(int methodId)
        {
            var methodEntity = _context.Methods
                .FirstOrDefault(m => m.MethodId == methodId);

            if (methodEntity != null)
            {
                _context.Methods.Remove(methodEntity);
                _context.SaveChanges();
            }
        }

        // Обновить данные метода
        public void UpdateMethod(int methodId, string methodName, int startLine, int endLine)
        {
            var methodEntity = _context.Methods.FirstOrDefault(m => m.MethodId == methodId);

            if (methodEntity != null)
            {
                methodEntity.MethodName = methodName;
                methodEntity.StartLine = startLine;
                methodEntity.EndLine = endLine;
                _context.SaveChanges();
            }
        }

        public List<Method> GetMethodsByClassId(int classId)
        {
            return _context.Methods
                .Where(m => m.ClassId == classId)
                .ToList();
        }

    }
}
