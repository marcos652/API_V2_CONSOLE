using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace FabricaDeProjeto.Api.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        // GET api/templates
        // Suporta filtros por query string: cliente, plano, status
        [HttpGet]
        public IActionResult GetAll([FromQuery] string cliente = "", [FromQuery] string plano = "", [FromQuery] string status = "")
        {
            var query = InMemoryDatabase.Templates.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(cliente))
            {
                query = query.Where(t => string.Equals(t.Cliente ?? string.Empty, cliente, StringComparison.OrdinalIgnoreCase)
                                      || (t.Cliente ?? string.Empty).IndexOf(cliente, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(plano))
            {
                query = query.Where(t => string.Equals(t.Plano ?? string.Empty, plano, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => string.Equals(t.Status ?? string.Empty, status, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.OrderBy(t => t.Nome).ToList();
            return Ok(list);
        }

        // GET api/templates/{id}
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var t = InMemoryDatabase.Templates.FirstOrDefault(x => x.Id == id);
            if (t == null) return NotFound(new { message = $"Template com id {id} não encontrado." });
            return Ok(t);
        }

        // GET api/templates/count?cliente=Empresa%20A
        [HttpGet("count")]
        public IActionResult Count([FromQuery] string cliente = "")
        {
            if (string.IsNullOrWhiteSpace(cliente))
                return Ok(new { total = InMemoryDatabase.Templates.Count });

            var total = InMemoryDatabase.Templates.Count(x => string.Equals(x.Cliente ?? string.Empty, cliente, StringComparison.OrdinalIgnoreCase));
            return Ok(new { cliente, total });
        }

        // GET api/templates/clients
        // Retorna lista única de clientes que possuem templates (útil para popular filtros no front)
        [HttpGet("clients")]
        public IActionResult GetClients()
        {
            var clients = InMemoryDatabase.Templates
                           .Select(t => t.Cliente ?? string.Empty)
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .OrderBy(s => s)
                           .ToList();
            return Ok(clients);
        }

        // POST api/templates
        // Cria um novo template. Impõe limite de 5 templates por cliente (como no front-end).
        [HttpPost]
        public IActionResult Create([FromBody] TemplateCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Dados do template são obrigatórios." });

            if (string.IsNullOrWhiteSpace(dto.Cliente))
                return BadRequest(new { message = "Cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(new { message = "Nome do template é obrigatório." });

            // Contagem por cliente
            var countForClient = InMemoryDatabase.Templates.Count(x => string.Equals(x.Cliente ?? string.Empty, dto.Cliente, StringComparison.OrdinalIgnoreCase));
            const int MAX_TEMPLATES_PER_CLIENT = 5;
            if (countForClient >= MAX_TEMPLATES_PER_CLIENT)
            {
                return BadRequest(new { message = $"Limite máximo de {MAX_TEMPLATES_PER_CLIENT} templates atingido para o cliente {dto.Cliente}." });
            }

            var tpl = new TemplateRecord
            {
                Id = InMemoryDatabase.NextTemplateId++,
                Cliente = dto.Cliente,
                Nome = dto.Nome,
                Plano = dto.Plano ?? "Básico",
                Status = dto.Status ?? "Ativo"
            };

            InMemoryDatabase.Templates.Add(tpl);

            // Retorna 201 com local do recurso criado
            return CreatedAtAction(nameof(Get), new { id = tpl.Id }, tpl);
        }

        // DELETE api/templates/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var t = InMemoryDatabase.Templates.FirstOrDefault(x => x.Id == id);
            if (t == null) return NotFound(new { message = $"Template com id {id} não encontrado." });

            InMemoryDatabase.Templates.Remove(t);
            return NoContent();
        }
    }

    // DTO para criação (pode ser ampliado futuramente)
    public class TemplateCreateDto
    {
        public string Cliente { get; set; }
        public string Nome { get; set; }
        public string Plano { get; set; }
        public string Status { get; set; }
    }
}