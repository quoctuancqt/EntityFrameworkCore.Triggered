﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveStartingTrigger
    {
        Task AfterSaveStarting(CancellationToken cancellationToken);
    }
}