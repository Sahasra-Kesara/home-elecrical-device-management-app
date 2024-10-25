using home_device_management_application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace home_device_management_application.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all devices
        public async Task<IActionResult> Index()
        {
            var devices = await _context.Devices.ToListAsync();
            return View(devices);
        }

        // Show details of a device
        public async Task<IActionResult> Details(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // Create a new device (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create a new device (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,ProductImage,Brand,BuyDate,WarrantyPeriod,ServicePeriod,CustomerServiceMobileNumber,LastServiceDate,RepairDueDate")] Device device, IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    // Save the uploaded image to a folder on the server
                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(stream);
                    }

                    // Store the image path in the device
                    device.ProductImage = "/images/" + fileName;
                }

                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // Edit device (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // Edit device (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductImage,Brand,BuyDate,WarrantyPeriod,ServicePeriod,CustomerServiceMobileNumber,LastServiceDate,RepairDueDate")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Devices.Any(e => e.Id == id))
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
            return View(device);
        }

        // Delete device (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // Delete device (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Search functionality
        public async Task<IActionResult> Search(string searchTerm)
        {
            var devices = from d in _context.Devices select d;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                devices = devices.Where(d => d.ProductName.Contains(searchTerm) || d.Brand.Contains(searchTerm));
            }

            return View("Index", await devices.ToListAsync());
        }

        // AddDevice View (GET)
        [HttpGet]
        [Route("Device/AddDevice")]
        public async Task<IActionResult> AddDevice()
        {
            var devices = await _context.Devices.ToListAsync();
            var model = new Tuple<Device, IEnumerable<Device>>(new Device(), devices);
            return View(model);
        }

        // AddDevice with image upload (POST)
        [HttpPost]
        [Route("Device/AddDevice")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDevice(Device device, IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    // Save the uploaded image to a folder on the server
                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(stream);
                    }

                    // Store the image path in the device
                    device.ProductImage = "/images/" + fileName;
                }

                // Add the new device to the database
                _context.Add(device);
                await _context.SaveChangesAsync();

                // Redirect to the Index action to show the list of devices
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, return the AddDevice view with the existing data
            var devices = await _context.Devices.ToListAsync();
            var model = new Tuple<Device, IEnumerable<Device>>(device, devices);
            return View(model);
        }
    }
}
