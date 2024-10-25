using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    public IActionResult AdminDashboard()
    {
        return View();
    }

    public IActionResult UserDashboard()
    {
        return View();
    }
}
