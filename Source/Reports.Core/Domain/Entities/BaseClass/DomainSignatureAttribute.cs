﻿using System;

namespace Reports.Core.Domain.Entities
{
    /// <summary>
    ///     Facilitates indicating which property(s) describe the unique signature of an 
    ///     entity.  See EntityWithTypedId.GetTypeSpecificSignatureProperties() for when this is leveraged.
    /// </summary>
    /// <remarks>
    ///     This is intended for use with <see cref = "EntityWithTypedId" />.
    /// </remarks>
    [Serializable]
    public class DomainSignatureAttribute : Attribute { }
}