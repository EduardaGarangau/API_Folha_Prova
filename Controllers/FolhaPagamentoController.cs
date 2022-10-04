using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/folha")]

    public class FolhaPagamentoController : ControllerBase
    {
        private readonly DataContext _context;

        public FolhaPagamentoController(DataContext context) => _context = context; 

        [HttpPost]
        [Route("cadastrar")]
        public IActionResult Cadastrar([FromBody] FolhaPagamento folha) {
            folha.SalarioBruto = folha.QuantidadeHoras * folha.ValorHora;  
            folha.ImpostoRenda = (folha.SalarioBruto * 0.275) - 869.36;
            folha.ImpostoInss = folha.SalarioBruto * 0.11; 
            folha.ImpostoFgts = folha.SalarioBruto * 0.08; 
            folha.SalarioLiquido = folha.SalarioBruto - folha.ImpostoRenda - folha.ImpostoInss; 
            folha.Mes = DateTime.Now.Month; 
            folha.Ano = DateTime.Now.Year; 
            Funcionario funcionario = _context.Funcionarios.Find(folha.FuncionarioId); 
            folha.Funcionario = funcionario; 

            if(funcionario != null) {
                _context.Folhas.Add(folha); 
                _context.SaveChanges(); 
                return Created("", folha);  
            } else {
                return NotFound(); 
            }
        }

        [HttpGet]
        [Route("listar")]
        public IActionResult Listar() {
            List<FolhaPagamento> folhas = new List<FolhaPagamento>(); 
            List<FolhaPagamento> folhasDB = _context.Folhas.ToList(); 

            foreach(var folha in folhasDB) {
                Funcionario funcionario = _context.Funcionarios.Find(folha.FuncionarioId); 
                folha.Funcionario = funcionario; 

                folhas.Add(folha); 
            }

           if(folhasDB == null) {
               return NotFound();
           } else {
               return Ok(folhas); 
           }
        }

        [HttpGet]
        [Route("buscar/{cpf}/{mes}/{ano}")]
        public IActionResult Buscar([FromRoute] string cpf, [FromRoute] int mes, [FromRoute] int ano) {
            Funcionario funcionario = _context.Funcionarios.FirstOrDefault(f => f.Cpf.Equals(cpf)); 
            FolhaPagamento folha = _context.Folhas.FirstOrDefault(f => f.FuncionarioId.Equals(funcionario.Id)); 

            if(mes == folha.Mes && ano == folha.Ano) {
                return Ok(folha);
            } else {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("filtrar/{mes}/{ano}")]
        public IActionResult Filtrar([FromRoute] int mes, [FromRoute] int ano) {
            List<FolhaPagamento> folhas = new List<FolhaPagamento>(); 
            List<FolhaPagamento> folhasDB = _context.Folhas.ToList();

            foreach(var folha in folhasDB) {
                Funcionario funcionario = _context.Funcionarios.Find(folha.FuncionarioId); 
                folha.Funcionario = funcionario; 
                
                if(mes == folha.Mes && ano == folha.Ano) {
                    folhas.Add(folha); 
                }
            }

            if(folhas == null) {
                return NotFound(); 
            } else {
                return Ok(folhas); 
            }

        }

    }
    
}