using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Productimages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;
        private readonly IManageProductService _manageProductService;

        public ProductsController(IPublicProductService publicProductService,
            IManageProductService manageProductService)
        {
            _publicProductService = publicProductService;
            _manageProductService = manageProductService;
        }
        [HttpPost()]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var productId = await _manageProductService.Create( request);
            if (productId == 0)
                return BadRequest();

            var product = await _manageProductService.GetById(productId, request.LanguageId);

            return CreatedAtAction(nameof(GetImageById), new { id = productId }, product);
        }

        //http://localhost:port/products?pageIndex=1&pageSize=10&CategoryId=
        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        {
            var products = await _publicProductService.GetAllByCategoryId(languageId, request);
            return Ok(products);
        }

        //http://localhost:port/product/1
        [HttpGet("{productid}/{languageId}")]
        public async Task<IActionResult> GetById(int productid, string languageId)
        {
            var product = await _manageProductService.GetById(productid, languageId);
            if (product == null)
                return BadRequest("Cannot find product");
            return Ok(product);
        }

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId,[FromForm] ProductImageCreateRequest request)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var imageId = await _manageProductService.AddImage(productId,request);
            if (imageId == 0)
                return BadRequest();

            var image = await _manageProductService.GetImageById(imageId);

            return CreatedAtAction(nameof(GetImageById), new { id = productId }, image);
        }
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var result = await _manageProductService.UpdateImage(imageId, request);
            if (result == 0)
                return BadRequest();


            return Ok();
        }
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var result = await _manageProductService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();


            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var affectedResult = await _manageProductService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{productid}")]
        public async Task<IActionResult> Delete(int productid)
        {
            var affectedResult = await _manageProductService.Delete(productid);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpPatch("{productid}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productid, decimal newPrice)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);
            var isSuccessful = await _manageProductService.UpdatePrice(productid, newPrice);
            if (isSuccessful)
                return Ok();

            return BadRequest();
        }
        [HttpGet("{productid}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int productid, int imageId)
        {
            var image = await _manageProductService.GetImageById( imageId);
            if (image == null)
                return BadRequest("Cannot find product");
            return Ok(image);
        }
    }
}