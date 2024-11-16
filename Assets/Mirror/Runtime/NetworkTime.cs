using System;
<<<<<<< Updated upstream
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mirror
{
    /// <summary>
    /// Synchronize time between the server and the clients
    /// </summary>
    public static class NetworkTime
    {
        /// <summary>
        /// how often are we sending ping messages
        /// used to calculate network time and RTT
        /// </summary>
        public static float PingFrequency = 2.0f;

        /// <summary>
        /// average out the last few results from Ping
        /// </summary>
=======
using System.Runtime.CompilerServices;
using UnityEngine;
#if !UNITY_2020_3_OR_NEWER
using Stopwatch = System.Diagnostics.Stopwatch;
#endif

namespace Mirror
{
    /// <summary>Synchronizes server time to clients.</summary>
    public static class NetworkTime
    {
        /// <summary>Ping message frequency, used to calculate network time and RTT</summary>
        public static float PingFrequency = 2.0f;

        /// <summary>Average out the last few results from Ping</summary>
>>>>>>> Stashed changes
        public static int PingWindowSize = 10;

        static double lastPingTime;

<<<<<<< Updated upstream
        // Date and time when the application started
        static readonly Stopwatch stopwatch = new Stopwatch();

        static NetworkTime()
        {
            stopwatch.Start();
        }

=======
>>>>>>> Stashed changes
        static ExponentialMovingAverage _rtt = new ExponentialMovingAverage(10);
        static ExponentialMovingAverage _offset = new ExponentialMovingAverage(10);

        // the true offset guaranteed to be in this range
        static double offsetMin = double.MinValue;
        static double offsetMax = double.MaxValue;

<<<<<<< Updated upstream
        // returns the clock time _in this system_
        static double LocalTime()
        {
            return stopwatch.Elapsed.TotalSeconds;
        }

        public static void Reset()
        {
=======
        /// <summary>Returns double precision clock time _in this system_, unaffected by the network.</summary>
#if UNITY_2020_3_OR_NEWER
        public static double localTime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Time.timeAsDouble;
        }
#else
        // need stopwatch for older Unity versions, but it's quite slow.
        // CAREFUL: unlike Time.time, this is not a FRAME time.
        //          it changes during the frame too.
        static readonly Stopwatch stopwatch = new Stopwatch();
        static NetworkTime() => stopwatch.Start();
        public static double localTime => stopwatch.Elapsed.TotalSeconds;
#endif

        /// <summary>The time in seconds since the server started.</summary>
        //
        // I measured the accuracy of float and I got this:
        // for the same day,  accuracy is better than 1 ms
        // after 1 day,  accuracy goes down to 7 ms
        // after 10 days, accuracy is 61 ms
        // after 30 days , accuracy is 238 ms
        // after 60 days, accuracy is 454 ms
        // in other words,  if the server is running for 2 months,
        // and you cast down to float,  then the time will jump in 0.4s intervals.
        //
        // TODO consider using Unbatcher's remoteTime for NetworkTime
        public static double time
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => localTime - _offset.Value;
        }

        /// <summary>Time measurement variance. The higher, the less accurate the time is.</summary>
        // TODO does this need to be public? user should only need NetworkTime.time
        public static double timeVariance => _offset.Var;

        /// <summary>Time standard deviation. The highe, the less accurate the time is.</summary>
        // TODO does this need to be public? user should only need NetworkTime.time
        public static double timeStandardDeviation => Math.Sqrt(timeVariance);

        /// <summary>Clock difference in seconds between the client and the server. Always 0 on server.</summary>
        public static double offset => _offset.Value;

        /// <summary>Round trip time (in seconds) that it takes a message to go client->server->client.</summary>
        public static double rtt => _rtt.Value;

        /// <summary>Round trip time variance. The higher, the less accurate the rtt is.</summary>
        // TODO does this need to be public? user should only need NetworkTime.time
        public static double rttVariance => _rtt.Var;

        /// <summary>Round trip time standard deviation. The higher, the less accurate the rtt is.</summary>
        // TODO does this need to be public? user should only need NetworkTime.time
        public static double rttStandardDeviation => Math.Sqrt(rttVariance);

        // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void ResetStatics()
        {
            PingFrequency = 2.0f;
            PingWindowSize = 10;
            lastPingTime = 0;
>>>>>>> Stashed changes
            _rtt = new ExponentialMovingAverage(PingWindowSize);
            _offset = new ExponentialMovingAverage(PingWindowSize);
            offsetMin = double.MinValue;
            offsetMax = double.MaxValue;
<<<<<<< Updated upstream
=======
#if !UNITY_2020_3_OR_NEWER
            stopwatch.Restart();
#endif
>>>>>>> Stashed changes
        }

        internal static void UpdateClient()
        {
<<<<<<< Updated upstream
            if (Time.time - lastPingTime >= PingFrequency)
            {
                NetworkPingMessage pingMessage = new NetworkPingMessage(LocalTime());
                NetworkClient.Send(pingMessage, Channels.DefaultUnreliable);
                lastPingTime = Time.time;
=======
            // localTime (double) instead of Time.time for accuracy over days
            if (localTime - lastPingTime >= PingFrequency)
            {
                NetworkPingMessage pingMessage = new NetworkPingMessage(localTime);
                NetworkClient.Send(pingMessage, Channels.Unreliable);
                lastPingTime = localTime;
>>>>>>> Stashed changes
            }
        }

        // executed at the server when we receive a ping message
        // reply with a pong containing the time from the client
        // and time from the server
<<<<<<< Updated upstream
        internal static void OnServerPing(NetworkConnection conn, NetworkPingMessage msg)
        {
            // Debug.Log("OnPingServerMessage  conn=" + conn);

            NetworkPongMessage pongMsg = new NetworkPongMessage
            {
                clientTime = msg.clientTime,
                serverTime = LocalTime()
            };

            conn.Send(pongMsg, Channels.DefaultUnreliable);
=======
        internal static void OnServerPing(NetworkConnectionToClient conn, NetworkPingMessage message)
        {
            // Debug.Log($"OnPingServerMessage conn:{conn}");
            NetworkPongMessage pongMessage = new NetworkPongMessage
            {
                clientTime = message.clientTime,
                serverTime = localTime
            };
            conn.Send(pongMessage, Channels.Unreliable);
>>>>>>> Stashed changes
        }

        // Executed at the client when we receive a Pong message
        // find out how long it took since we sent the Ping
        // and update time offset
<<<<<<< Updated upstream
        internal static void OnClientPong(NetworkPongMessage msg)
        {
            double now = LocalTime();

            // how long did this message take to come back
            double newRtt = now - msg.clientTime;
=======
        internal static void OnClientPong(NetworkPongMessage message)
        {
            double now = localTime;

            // how long did this message take to come back
            double newRtt = now - message.clientTime;
>>>>>>> Stashed changes
            _rtt.Add(newRtt);

            // the difference in time between the client and the server
            // but subtract half of the rtt to compensate for latency
            // half of rtt is the best approximation we have
<<<<<<< Updated upstream
            double newOffset = now - newRtt * 0.5f - msg.serverTime;

            double newOffsetMin = now - newRtt - msg.serverTime;
            double newOffsetMax = now - msg.serverTime;
=======
            double newOffset = now - newRtt * 0.5f - message.serverTime;

            double newOffsetMin = now - newRtt - message.serverTime;
            double newOffsetMax = now - message.serverTime;
>>>>>>> Stashed changes
            offsetMin = Math.Max(offsetMin, newOffsetMin);
            offsetMax = Math.Min(offsetMax, newOffsetMax);

            if (_offset.Value < offsetMin || _offset.Value > offsetMax)
            {
                // the old offset was offrange,  throw it away and use new one
                _offset = new ExponentialMovingAverage(PingWindowSize);
                _offset.Add(newOffset);
            }
            else if (newOffset >= offsetMin || newOffset <= offsetMax)
            {
                // new offset looks reasonable,  add to the average
                _offset.Add(newOffset);
            }
        }
<<<<<<< Updated upstream

        /// <summary>
        /// The time in seconds since the server started.
        /// </summary>
        //
        // I measured the accuracy of float and I got this:
        // for the same day,  accuracy is better than 1 ms
        // after 1 day,  accuracy goes down to 7 ms
        // after 10 days, accuracy is 61 ms
        // after 30 days , accuracy is 238 ms
        // after 60 days, accuracy is 454 ms
        // in other words,  if the server is running for 2 months,
        // and you cast down to float,  then the time will jump in 0.4s intervals.
        public static double time => LocalTime() - _offset.Value;

        /// <summary>
        /// Measurement of the variance of time.
        /// <para>The higher the variance, the less accurate the time is</para>
        /// </summary>
        public static double timeVar => _offset.Var;

        /// <summary>
        /// standard deviation of time.
        /// <para>The higher the variance, the less accurate the time is</para>
        /// </summary>
        public static double timeSd => Math.Sqrt(timeVar);

        /// <summary>
        /// Clock difference in seconds between the client and the server
        /// </summary>
        /// <remarks>
        /// Note this value is always 0 at the server
        /// </remarks>
        public static double offset => _offset.Value;

        /// <summary>
        /// how long in seconds does it take for a message to go
        /// to the server and come back
        /// </summary>
        public static double rtt => _rtt.Value;

        /// <summary>
        /// measure variance of rtt
        /// the higher the number,  the less accurate rtt is
        /// </summary>
        public static double rttVar => _rtt.Var;

        /// <summary>
        /// Measure the standard deviation of rtt
        /// the higher the number,  the less accurate rtt is
        /// </summary>
        public static double rttSd => Math.Sqrt(rttVar);
=======
>>>>>>> Stashed changes
    }
}
