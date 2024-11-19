using Microsoft.EntityFrameworkCore;
using BDwAS_projekt.Models;

namespace BDwAS_projekt.Data;

//Interfejs który muszą implementować wszystkie bazy danych.
public interface IDbContext
{
    public List<User> GetUsers(); //Zwraca tylko użytkowników, nie musi zwracać subskrypcji, Payments, ViewedPosts
    public User GetUser(string id); //Zwraca pełnego użytkownika z subskrypcjami, Payments, ViewedPosts
    public void AddUsers(User user);
    public void DeleteUser(string id);

    public List<Channel> GetChannels();
    //...
}