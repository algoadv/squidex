// ==========================================================================
//  Schema.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Core.Schemas
{
    public sealed class Schema : ImmutableDomainObject
    {
        private readonly string name;
        private SchemaProperties properties = new SchemaProperties();
        private ImmutableList<Field> fields = ImmutableList<Field>.Empty;
        private ImmutableDictionary<long, Field> fieldsById;
        private ImmutableDictionary<string, Field> fieldsByName;
        private bool isPublished;

        public string Name
        {
            get { return name; }
        }

        public bool IsPublished
        {
            get { return isPublished; }
        }

        public ImmutableList<Field> Fields
        {
            get { return fields; }
        }

        public ImmutableDictionary<long, Field> FieldsById
        {
            get { return fieldsById; }
        }

        public ImmutableDictionary<string, Field> FieldsByName
        {
            get { return fieldsByName; }
        }

        public SchemaProperties Properties
        {
            get { return properties; }
        }

        private Schema(Guid id, Instant now, RefToken actor, string name, SchemaProperties properties)
            : base(id, now, actor)
        {
            this.name = name;

            this.properties = properties;
        }

        public override void OnInit()
        {
            fieldsById = fields.ToImmutableDictionary(x => x.Id);
            fieldsByName = fields.ToImmutableDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            properties.Freeze();
        }

        public static Schema Create(Guid id, Instant now, RefToken actor, string name, SchemaProperties properties)
        {
            return new Schema(id, now, actor, name, properties);
        }

        public Schema Update(SchemaProperties newProperties)
        {
            Guard.NotNull(newProperties, nameof(newProperties));

            return new Schema(name, isPublished, newProperties, fields);
        }

        public Schema UpdateField(long fieldId, FieldProperties newProperties)
        {
            return UpdateField(fieldId, field => field.Update(newProperties));
        }

        public Schema LockField(long fieldId)
        {
            return UpdateField(fieldId, field => field.Lock());
        }

        public Schema DisableField(long fieldId)
        {
            return UpdateField(fieldId, field => field.Disable());
        }

        public Schema EnableField(long fieldId)
        {
            return UpdateField(fieldId, field => field.Enable());
        }

        public Schema HideField(long fieldId)
        {
            return UpdateField(fieldId, field => field.Hide());
        }

        public Schema ShowField(long fieldId)
        {
            return UpdateField(fieldId, field => field.Show());
        }

        public Schema Publish()
        {
            return new Schema(name, true, properties, fields);
        }

        public Schema Unpublish()
        {
            return new Schema(name, false, properties, fields);
        }

        public Schema DeleteField(long fieldId)
        {
            var newFields = fields.Where(x => x.Id != fieldId).ToImmutableList();

            return new Schema(name, isPublished, properties, newFields);
        }

        public Schema UpdateField(long fieldId, Func<Field, Field> updater)
        {
            Guard.NotNull(updater, nameof(updater));

            var newFields = fields.Select(f => f.Id == fieldId ? updater(f) ?? f : f).ToImmutableList();

            return new Schema(name, isPublished, properties, newFields);
        }

        public Schema ReorderFields(List<long> ids)
        {
            Guard.NotNull(ids, nameof(ids));

            if (ids.Count != fields.Count || ids.Any(x => !fieldsById.ContainsKey(x)))
            {
                throw new ArgumentException("Ids must cover all fields.", nameof(ids));
            }

            var newFields = fields.OrderBy(f => ids.IndexOf(f.Id)).ToImmutableList();

            return new Schema(name, isPublished, properties, newFields);
        }

        public Schema AddField(Field field)
        {
            Guard.NotNull(field, nameof(field));

            if (fieldsByName.ContainsKey(field.Name) || fieldsById.ContainsKey(field.Id))
            {
                throw new ArgumentException($"A field with name '{field.Name}' already exists.", nameof(field));
            }

            var newFields = fields.Add(field);

            return new Schema(name, isPublished, properties, newFields);
        }
    }
}