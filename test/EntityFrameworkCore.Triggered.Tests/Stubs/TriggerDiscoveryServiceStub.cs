﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerDiscoveryServiceStub : ITriggerDiscoveryService
    {
        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
            => Enumerable.Empty<TriggerDescriptor>();

        public void SetServiceProvider(IServiceProvider serviceProvider) { }
    }
}
