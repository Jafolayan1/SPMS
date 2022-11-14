﻿using AspNetCoreHero.ToastNotification.Abstractions;

using AutoMapper;

using CPMS.Hubs;

using Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CPMS.Areas.Supervisors.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly IHubContext<MessageHub> _hubContext;

        public ProjectController(IUserAccessor userAccessor, IUnitOfWork context, IMapper mapper, IMailService mail, INotyfService notyf, IHubContext<MessageHub> hubContext) : base(userAccessor, context, mail)
        {
            _mapper = mapper;
            _notyf = notyf;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult PStudent()
        {
            var supervisor = _context.Supervisors.GetById(CurrentUser.UserName);
            ViewData["supervisor"] = supervisor;
            return View();
        }

        [HttpGet]
        public IActionResult Proposal()
        {
            var supervisor = _context.Supervisors.GetById(CurrentUser.UserName);
            var lstProposal = _context.Projects.Find(x => x.SupervisorId.Equals(supervisor.SupervisorId), false).Where(s => s.Status.Equals("Pending"));
            ViewData["projectProposal"] = lstProposal;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Status(int ProjectId, string item)
        {
            try
            {
                var project = _context.Projects.GetById(ProjectId);
                project.Status = item;
                _context.Projects.Update(project);
                await _context.SaveAsync();
                SendMail();
                return RedirectToAction(nameof(Proposal));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return RedirectToAction(nameof(Proposal));
            }
        }
    }
}