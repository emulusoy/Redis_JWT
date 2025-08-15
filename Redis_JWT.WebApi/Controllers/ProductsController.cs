using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Redis_JWT.Application.Features.Products.Commands.CreateCommands;
using Redis_JWT.Application.Features.Products.Commands.DeleteCommands;
using Redis_JWT.Application.Features.Products.Commands.UpdateCommands;
using Redis_JWT.Application.Features.Products.Queries.GetAll;
using Redis_JWT.Application.Features.Products.Queries.GetByIdQuery;

namespace Redis_JWT.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var res = await mediator.Send(new GetAllProductsQuery());
            return res.IsSuccess ? Ok(res.Value) : BadRequest(new { error = res.Error });
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await mediator.Send(new GetProductByIdQuery(id));
            return res.IsSuccess ? Ok(res.Value) : NotFound(new { error = res.Error });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
        {
            var res = await mediator.Send(cmd, ct);
            return res.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = res.Value }, new { id = res.Value })
                : BadRequest(new { error = res.Error });
        }
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateProductCommand body)
        {
            var cmd = body with { Id = id };
            var res = await mediator.Send(cmd);
            return res.IsSuccess ? NoContent() : NotFound(new { error = res.Error });
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await mediator.Send(new DeleteProductCommand(id));
            return res.IsSuccess ? NoContent() : BadRequest(new { error = res.Error });
        }
    }
}
