# Release channels in Starcounter

Starcounter supports a number of different release [channels](http://downloads.starcounter.com/download). We use these channels to roll out updates to users, starting with our close to hourly builds, all the way up to our Release channel releases.

### Types of releases:

### Release

Stability: **High**, Frequency: **Low**

This channel contains releases that have been thoroughly tested by the Starcounter team and our clients. The latest release on this channel is used for the master branch of our [sample apps](https://github.com/starcounterapps). For most purposes, using the latest release from this channel is recommended. It's updated approximately once every six months.

### Release Candidate

Stability: **Medium**, Frequency: **High**

This channel is for versions that are stable enough to be considered for release. It's updated daily with bug fixes. We don't introduce new functionality in the Release Candidate, that's done on the Develop channel.

### Develop

Stability: **Low**, Frequency: **High**

This channel is the unstable iterated release channel. It's updated every night after a full day of work. Features from the Develop release are most probable to appear in the next “Release” version. Feel free to use it for the bleeding edge developer experience.

### Custom

Stability: **Varied**, Frequency: **Varied**

This channel is for custom builds that are used for specific purposes. It may, for example, help to detect problems in an environment. Releases of this channel should mainly be used by developers who are explicitly directed to them by our support.

### Long-term support \(LTS\)

Stability: **High**, Frequency: **Low-Medium**

This channel is updated nightly when there are hotfixes and patches for the latest release of the Release channel. There will not be a major release in this channel unless there is also a release in the Release channel, thus, this channel and the Release channel does not differ in functionality.

