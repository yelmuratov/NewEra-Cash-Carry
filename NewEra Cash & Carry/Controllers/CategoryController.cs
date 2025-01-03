using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.category;
using NewEra_Cash___Carry.Models;

namespace NewEra_Cash___Carry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;
        public CategoryController(RetailOrderingSystemDbContext context)
        { 
            _context = context;
        }

        //Get Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>>GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        //Get Product By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {   
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        //Post category
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] CategoryPostDto categoryDto)
        {
            if (await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name))
            {
                return Conflict(new { message = "A category with this name already exists." });
            }

            var category = new Category
            {
                Name = categoryDto.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        //Update Category
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryDto.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Delete Category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(); 

            return NoContent();
        }

    }
}
