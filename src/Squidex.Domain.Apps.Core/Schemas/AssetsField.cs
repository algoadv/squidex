﻿// ==========================================================================
//  AssetsField.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Squidex.Domain.Apps.Core.Schemas.Validators;

namespace Squidex.Domain.Apps.Core.Schemas
{
    public sealed class AssetsField : Field<AssetsFieldProperties>, IReferenceField
    {
        private static readonly Guid[] EmptyIds = new Guid[0];

        public AssetsField(long id, string name, Partitioning partitioning)
            : this(id, name, partitioning, new AssetsFieldProperties())
        {
        }

        public AssetsField(long id, string name, Partitioning partitioning, AssetsFieldProperties properties)
            : base(id, name, partitioning, properties)
        {
        }

        protected override IEnumerable<IValidator> CreateValidators()
        {
            yield return new AssetsValidator(Properties.IsRequired, Properties.MinItems, Properties.MaxItems);
        }

        public IEnumerable<Guid> GetReferencedIds(JToken value)
        {
            Guid[] assetIds;
            try
            {
                assetIds = value?.ToObject<Guid[]>() ?? EmptyIds;
            }
            catch
            {
                assetIds = EmptyIds;
            }

            return assetIds;
        }

        public JToken RemoveDeletedReferences(JToken value, ISet<Guid> deletedReferencedIds)
        {
            if (value == null || value.Type == JTokenType.Null)
            {
                return null;
            }

            var oldAssetIds = GetReferencedIds(value).ToArray();
            var newAssetIds = oldAssetIds.Where(x => !deletedReferencedIds.Contains(x)).ToList();

            return newAssetIds.Count != oldAssetIds.Length ? JToken.FromObject(newAssetIds) : value;
        }

        public override object ConvertValue(JToken value)
        {
            return new AssetsValue(value.ToObject<Guid[]>());
        }

        public override T Visit<T>(IFieldVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
