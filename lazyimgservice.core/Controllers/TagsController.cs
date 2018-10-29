using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lazyimgservice.core.Models;
using System.ComponentModel.DataAnnotations;

namespace lazyimgservice.core.Controllers
{
    [Produces("application/json")]
    [Route("api/Tags")]
    public class TagsController : Controller
    {
        private readonly EFContext _context;

        public TagsController(EFContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all available tags
        /// </summary>        
        [HttpGet]
        public IEnumerable<string> GetTags()
        {
            return _context.Tags.Select(x=>x.Name);
        }

        /// <summary>
        /// Update a tag
        /// </summary>
        /// <param name="oldTag">The tag to replace</param>
        /// <param name="newTag">The new tag name</param>
        [HttpPut("{oldTag}")]
        public async Task<IActionResult> PutTag([FromRoute] string oldTag, [FromQuery][Required]string newTag)
        {
            var tag= _context.Tags.FirstOrDefault(x => x.Name == oldTag.ToLower());           
            if (tag==null)
            {
                return NotFound();
            }

            tag.Name = newTag.ToLower();
            _context.Entry(tag).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        /// <summary>
        /// Delete a tag and all of its associated links
        /// </summary>
        /// <param name="tag">The tag to delete</param>
        [HttpDelete("{tag}")]
        public async Task<IActionResult> DeleteTag([FromRoute] string tag)
        {
            var entity = await _context.Tags.Include(x=>x.TagLinks).SingleOrDefaultAsync(m => m.Name == tag.ToLower());
            if (entity == null)
            {
                return NotFound();
            }

            foreach(var link in entity.TagLinks)
            {
                _context.TagLinks.Remove(link);
            }
            _context.Tags.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}