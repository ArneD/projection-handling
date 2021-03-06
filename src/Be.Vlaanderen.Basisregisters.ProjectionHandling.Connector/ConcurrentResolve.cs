namespace Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents the available concurrent <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolvers</see>.
    /// </summary>
    public static class ConcurrentResolve
    {
        /// <summary>
        /// Resolves the <see cref="ConnectedProjectionHandler{TConnection}">handlers</see> that match the type of the message exactly.
        /// </summary>
        /// <param name="handlers">The set of resolvable handlers.</param>
        /// <returns>A <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolver</see>.</returns>
        public static ConnectedProjectionHandlerResolver<TConnection> WhenEqualToHandlerMessageType<TConnection>(ConnectedProjectionHandler<TConnection>[] handlers)
            => Resolve.WhenEqualToHandlerMessageType(handlers);

        /// <summary>
        /// Resolves the <see cref="ConnectedProjectionHandler{TConnection}">handlers</see> to which the message instance is assignable.
        /// </summary>
        /// <param name="handlers">The set of resolvable handlers.</param>
        /// <returns>A <see cref="ConnectedProjectionHandlerResolver{TConnection}">resolver</see>.</returns>
        public static ConnectedProjectionHandlerResolver<TConnection> WhenAssignableToHandlerMessageType<TConnection>(ConnectedProjectionHandler<TConnection>[] handlers)
        {
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));

            var cache = new ConcurrentDictionary<Type, ConnectedProjectionHandler<TConnection>[]>();
            return message =>
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (!cache.TryGetValue(message.GetType(), out var result))
                    result = cache.GetOrAdd(message.GetType(), Array.FindAll(handlers, handler => handler.Message.IsInstanceOfType(message)));

                return result;
            };
        }
    }
}
