using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.ViewModels;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                // Buscar o Id no banco utilizando o EF
                var tarefa = await _context.Tarefas.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                // Validar o tipo de retorno. Se não encontrar a tarefa, retornar NotFound,
                if(tarefa == null)
                    return NotFound(new ResultViewModel<Tarefa>("Tarefa não encontrada"));

                // caso contrário retornar OK com a tarefa encontrada
                return Ok(new ResultViewModel<Tarefa>(tarefa));
            }
            catch
            {

                return StatusCode(500, new ResultViewModel<string>("05X01 - Falha interna no servidor"));
            }

        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                // Buscar todas as tarefas no banco utilizando o EF
                var tarefas = await _context.Tarefas.AsNoTracking().ToListAsync();

                return Ok(new ResultViewModel<List<Tarefa>>(tarefas));
            }
            catch
            {

                return StatusCode(500, new ResultViewModel<string>("05X02 - Falha interna no servidor"));
            }

        }

        [HttpGet("ObterPorTitulo")]
        public async Task<IActionResult> ObterPorTitulo(string titulo)
        {
            try
            {
                // Buscar  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
                var tarefas = await _context.Tarefas.AsNoTracking().Where(x => x.Titulo.Contains(titulo)).ToListAsync();

                return Ok(new ResultViewModel<List<Tarefa>>(tarefas));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X03 - Falha interna no servidor"));
            }

        }

        [HttpGet("ObterPorData")]
        public async Task<IActionResult> ObterPorData(DateTime data)
        {
            try
            {
                var tarefas = await _context.Tarefas.AsNoTracking().Where(x => x.Data.Date == data.Date).ToListAsync();

                return Ok(new ResultViewModel<List<Tarefa>>(tarefas));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("ObterPorStatus")]
        public async Task<IActionResult> ObterPorStatus(EnumStatusTarefa status)
        {
            try
            {
                // Buscar  as tarefas no banco utilizando o EF, que contenha o status recebido por parâmetro
                var tarefas = await _context.Tarefas.AsNoTracking().Where(x => x.Status == status).ToListAsync();

                return Ok(tarefas);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X05 - Falha interna no servidor"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            try
            {
                if (tarefa.Data == DateTime.MinValue)
                    return BadRequest(new ResultViewModel<string>("A data da tarefa não pode ser vazia"));

                await _context.Tarefas.AddAsync(tarefa);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X06 - Falha interna no servidor"));
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Tarefa tarefa)
        {
            try
            {
                var tarefaBanco = _context.Tarefas.Find(id);

                if (tarefaBanco == null)
                    return NotFound(new ResultViewModel<string>("Tarefa não encontrada"));

                if (tarefa.Data == DateTime.MinValue)
                    return BadRequest(new ResultViewModel<string>("A data da tarefa não pode ser vazia"));

                // Atualizar as informações da variável tarefaBanco com a tarefa recebida via parâmetro
                tarefaBanco.Titulo = tarefa.Titulo;
                tarefaBanco.Descricao = tarefa.Descricao;
                tarefaBanco.Data = tarefa.Data;
                tarefaBanco.Status = tarefa.Status;

                // Atualizar a variável tarefaBanco no EF e salvar as mudanças (save changes)
                _context.Tarefas.Update(tarefaBanco);
                await _context.SaveChangesAsync();

                return Ok(new ResultViewModel<Tarefa>(tarefaBanco));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X07 - Falha interna no servidor"));
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                var tarefaBanco = _context.Tarefas.Find(id);

                if (tarefaBanco == null)
                    return NotFound(new ResultViewModel<string>("Tarefa não encontrada"));

                // Remover a tarefa encontrada através do EF e salvar as mudanças (save changes)
                _context.Tarefas.Remove(tarefaBanco);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X08 - Falha interna no servidor"));
            }

        }
    }
}
