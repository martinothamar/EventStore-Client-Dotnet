using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client.Operations;

#nullable enable
namespace EventStore.Client {
	public partial class EventStoreOperationsClient {
		public async Task<DatabaseScavengeResult> StartScavengeAsync(
			int threadCount = 1,
			int startFromChunk = 0,
			UserCredentials? userCredentials = null,
			CancellationToken cancellationToken = default) {
			if (threadCount <= 0) {
				throw new ArgumentOutOfRangeException(nameof(threadCount));
			}

			if (startFromChunk < 0) {
				throw new ArgumentOutOfRangeException(nameof(startFromChunk));
			}

			var result = await _client.StartScavengeAsync(new StartScavengeReq {
					Options = new StartScavengeReq.Types.Options {
						ThreadCount = threadCount,
						StartFromChunk = startFromChunk
					}
				}, RequestMetadata.Create(userCredentials ?? Settings.DefaultCredentials),
				cancellationToken: cancellationToken);

			return result.ScavengeResult switch {
				ScavengeResp.Types.ScavengeResult.Started => DatabaseScavengeResult.Started(result.ScavengeId),
				ScavengeResp.Types.ScavengeResult.Stopped => DatabaseScavengeResult.Stopped(result.ScavengeId),
				ScavengeResp.Types.ScavengeResult.InProgress => DatabaseScavengeResult.InProgress(result.ScavengeId),
				_ => throw new InvalidOperationException()
			};
		}

		public async Task<DatabaseScavengeResult> StopScavengeAsync(
			string scavengeId,
			UserCredentials? userCredentials = null,
			CancellationToken cancellationToken = default) {
			var result = await _client.StopScavengeAsync(new StopScavengeReq {
					Options = new StopScavengeReq.Types.Options {
						ScavengeId = scavengeId
					}
				}, RequestMetadata.Create(userCredentials ?? Settings.DefaultCredentials),
				cancellationToken: cancellationToken);

			return result.ScavengeResult switch {
				ScavengeResp.Types.ScavengeResult.Started => DatabaseScavengeResult.Started(result.ScavengeId),
				ScavengeResp.Types.ScavengeResult.Stopped => DatabaseScavengeResult.Stopped(result.ScavengeId),
				ScavengeResp.Types.ScavengeResult.InProgress => DatabaseScavengeResult.InProgress(result.ScavengeId),
				_ => throw new InvalidOperationException()
			};
		}
	}
}
