﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsManagingSystem.Models;
using System.Drawing;

namespace EventsMS.Controllers;

public class UsersController : Controller
{
    private readonly EventsMSDBContext _context;

    public UsersController(EventsMSDBContext context)
    {
        _context = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,MobileNumber,Username,Password,Email,CreatedAt,Avatar")] User user, IFormFile? avatar)
    {
        if (ModelState.IsValid)
        {
            if (!await UsersController.IsUserExist(user.Name,
                                                   user.MobileNumber,
                                                   user.Username,
                                                   user.Email,
                                                   _context,
                                                   avatar))
            {
                if (avatar != null && avatar.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await avatar.CopyToAsync(memoryStream);
                        user.Avatar = memoryStream.ToArray();
                    }
                }
                else
                {
                    string imagePath = "C:\\Users\\Andrii\\source\\repos\\MoviePickerWebApplication_v2\\src\\MoviePickerMVC\\MoviePickerInfrastructure\\wwwroot\\Images\\no_movie_image.jpg";
                    if (System.IO.File.Exists(imagePath))
                    {
                        byte[] defaultImageBytes = System.IO.File.ReadAllBytes(imagePath);
                        user.Avatar = defaultImageBytes;
                    }
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } 
            else
            {
                ModelState.AddModelError(string.Empty, "This user already exists.");
            }
        }
        return View(user);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,MobileNumber,Username,Password,Email,CreatedAt,Avatar,Id")] User user, IFormFile? avatar)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (!await UsersController.IsUserExist(user.Name,
                                                   user.MobileNumber,
                                                   user.Username,
                                                   user.Email,
                                                   _context,
                                                   avatar))
            {
                try
                {


                    var existingUser = await _context.Users.FindAsync(id);

                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    _context.Entry(existingUser).State = EntityState.Detached;

                    if (avatar != null && avatar.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await avatar.CopyToAsync(memoryStream);
                            user.Avatar = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        user.Avatar = existingUser.Avatar;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "This user already exists.");
            }
        }
        return View(user);
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    public static async Task<bool> IsUserExist(string name,
                                               string mobileNumber,
                                               string username,
                                               string email,
                                               EventsMSDBContext context,
                                               IFormFile? avatar = null)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u =>
                                   u.Name == name &&
                                   u.MobileNumber == mobileNumber &&
                                   u.Username == username &&
                                   u.Email == email);


        byte[]? image = [];
        if (avatar != null && avatar.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await avatar.CopyToAsync(memoryStream);
                image = memoryStream.ToArray();
            }

            if (user != null && user.Avatar!.SequenceEqual(image))
            {
                return true;
            }

            return false;
        }
        else
        {
            return user != null;
        }
    }



    


}
