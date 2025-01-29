using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteManagementSystem.Models;
using NoteManagementSystem.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoteManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get()
        {
            var categories = await _categoryService.GetAsync(GetUserId());
            if (categories == null || categories.Count == 0)
            {
                return NotFound(new { Message = "No categories found." });
            }
            return Ok(categories);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Category>> Get(string id)
        {
            var category = await _categoryService.GetAsync(id, GetUserId());
            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            category.UserId = GetUserId();
            await _categoryService.CreateAsync(category);
            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Category categoryIn)
        {
            var category = await _categoryService.GetAsync(id, GetUserId());
            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            categoryIn.Id = category.Id;
            categoryIn.UserId = GetUserId();

            await _categoryService.UpdateAsync(id, categoryIn, GetUserId());
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var category = await _categoryService.GetAsync(id, GetUserId());
            if (category == null)
            {
                return NotFound(new { Message = "Category not found." });
            }

            await _categoryService.RemoveAsync(id, GetUserId());
            return NoContent();
        }
    }
}
