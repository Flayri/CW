using System.Collections.Generic;

namespace _1234
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetElementsList(); // получение всех объектов
        T GetElement(int id); // получение одного объекта по id
        void CreateElement(T item); // создание объекта
        void UpdateElement(T item); // обновление объекта
        void DeleteElement(T item); // удаление объекта по id
        void SaveElement();  // сохранение изменений
    }
}


