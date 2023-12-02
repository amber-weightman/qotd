﻿using Qotd.Application.Models;

namespace Qotd.Application.Interfaces;

public interface IQuestionService
{
    Task<Metadata> Setup(Metadata? metadata, CancellationToken cancellationToken);
    Task<RunResponse> GenerateQuestion(Metadata metadata, CancellationToken cancellationToken);
    Task<QuestionResponse> GetQuestion(Metadata metadata, CancellationToken cancellationToken);
}
