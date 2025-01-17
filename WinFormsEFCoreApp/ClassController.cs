using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsEFCoreApp;
using static WinFormsEFCoreApp.Program;

namespace WinFormsEFCoreApp.Controllers
{
    public class ClassController
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все классы
        public List<Class> GetClasses()
        {
            return _context.Classes
                .Include(c => c.Methods)  // Загрузка связанных методов
                .ToList();
        }

        // Получить класс по ID
        public Class GetClassById(int id)
        {
            return _context.Classes
                .Include(c => c.Methods)
                .FirstOrDefault(c => c.ClassId == id);
        }

        // Добавить новый класс
        public void AddClass(Class newClass)
        {
            _context.Classes.Add(newClass);
            _context.SaveChanges();
        }

        // Удалить класс по ID
        public void DeleteClass(int classId)
        {
            var classEntity = _context.Classes
                .Include(c => c.Methods)
                .FirstOrDefault(c => c.ClassId == classId);

            if (classEntity != null)
            {
                // Удаляем связанные методы
                _context.Methods.RemoveRange(classEntity.Methods);
                // Удаляем сам класс
                _context.Classes.Remove(classEntity);
                _context.SaveChanges();
            }
        }

        // Обновить данные класса
        public void UpdateClass(int classId, string className, int startLine, int endLine, int methodsCount)
        {
            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassId == classId);

            if (classEntity != null)
            {
                classEntity.ClassName = className;
                classEntity.StartLine = startLine;
                classEntity.EndLine = endLine;
                classEntity.MethodsCount = methodsCount;
                _context.SaveChanges();
            }
        }

        // Добавление класса с передачей параметров в метод
        public void AddClass(string className, int fileId, int startLine, int endLine, int methodsCount)
        {
            var classEntity = new Class
            {
                ClassName = className,
                FileId = fileId,
                StartLine = startLine,
                EndLine = endLine,
                MethodsCount = methodsCount
            };

            _context.Classes.Add(classEntity);
            _context.SaveChanges();
        }

        public List<Class> GetClassesByFileId(int fileId)
        {
            return _context.Classes
                .Where(c => c.FileId == fileId)
                .ToList();
        }

    }
}
