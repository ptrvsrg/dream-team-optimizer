<h1 align="center" style="font-weight: bold;">Dream Team Optimizer</h1>

<p align="center">
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/actions/workflows/build-and-test-dotnet.yml">
        <img alt="GitHub Actions Build and Test Workflow Status" src="https://img.shields.io/github/actions/workflow/status/ptrvsrg/dream-team-optimizer/build-and-test-dotnet.yml?branch=develop&style=flat&labelColor=222222&color=77D4FC&label=Build%20and%20Test%20%7C%20develop">
    </a>
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/graphs/contributors">
        <img alt="GitHub contributors" src="https://img.shields.io/github/contributors/ptrvsrg/dream-team-optimizer?style=flat&label=Contributors&labelColor=222222&color=77D4FC"/>
    </a>
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/forks">
        <img alt="GitHub forks" src="https://img.shields.io/github/forks/ptrvsrg/dream-team-optimizer?style=flat&label=Forks&labelColor=222222&color=77D4FC"/>
    </a>
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/stargazers">
        <img alt="GitHub Repo stars" src="https://img.shields.io/github/stars/ptrvsrg/dream-team-optimizer?style=flat&label=Stars&labelColor=222222&color=77D4FC"/>
    </a>
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/issues">
        <img alt="GitHub issues" src="https://img.shields.io/github/issues/ptrvsrg/dream-team-optimizer?style=flat&label=Issues&labelColor=222222&color=77D4FC"/>
    </a>
    <a href="https://github.com/ptrvsrg/dream-team-optimizer/pulls">
        <img alt="GitHub pull requests" src="https://img.shields.io/github/issues-pr/ptrvsrg/dream-team-optimizer?style=flat&label=Pull%20Requests&labelColor=222222&color=77D4FC"/>
    </a>
</p>

<p align="center">The algorithm for forming development teams based on their preferences, in order to maximize harmony and satisfaction of participants.</p>



<h2 id="description">Description</h2>

<details>
  <summary>More detailed description of the task</summary>

**The Dream Team Optimizer** project is designed to optimally form development teams based on their preferences gathered
during the hackathon. Each developer (Juniors or TeamLeads) makes a list of desirable colleagues with whom he would like
to work in a team. Based on this data, the project calculates the satisfaction index for each participant, and then
calculates the harmony of the team distribution. The main goal of the project is to maximize the harmony of team
formation in order to ensure the greatest satisfaction of the participants. This tool can be useful for HR professionals
to optimize the process of creating dream teams.

</details>

<h3>Strategies</h3>

+ Gale–Shapley algorithm
+ Strategy based on a complete bipartite graph (Gale–Shapley algorithm with additional rating points)

<h2 id="technologies">Technologies</h2>

+ .NET SDK 8.0

<h2 id="started">Getting started</h2>

<h3>Prerequisites</h3>

- Git
- .NET SDK
- Docker

<h3>Installation</h3>

```shell
git clone https://github.com/ptrvsrg/dream-team-optimizer
```

<h3>Launch</h3>

Help message:

```shell
DreamTeamOptimizer.ConsoleApp 0.0.0+f832d4867089b925e28d68c2c88097762ca11b21
Copyright (C) 2024 DreamTeamOptimizer.ConsoleApp

  -j, --juniors        Required. Path to the juniors CSV file.

  -t, --teamleads      Required. Path to the team leads CSV file.

  -s, --strategy       (Default: GaleShapley) Strategy to use for team building (GaleShapley, BipartiteGraphWithRating).

  -n, --hackathons     (Default: 1000) Number of hackathons to conduct.

  -c, --concurrency    (Default: 1) Number of threads.

  --help               Display this help screen.

  --version            Display version information.
```

<h4>Locally</h4>

```shell
make build.console
./out/console/DreamTeamOptimizer.ConsoleApp \
  -j DreamTeamOptimizer.ConsoleApp/example/Juniors20.csv \
  -t DreamTeamOptimizer.ConsoleApp/example/Teamleads20.csv \
  -s GaleShapley \
  -c 4
```

<h4>Docker</h4>

```shell
make build-image.console
docker run -v "./DreamTeamOptimizer.ConsoleApp/example:/example" ptrvsrg/dream-team-optimizer-console \
  -j /example/Juniors20.csv \
  -t /example/Teamleads20.csv \
  -s GaleShapley \
  -c 4
```

<h2 id="contribute">Contribute</h2>

See in the [CONTRIBUTING.md](CONTRIBUTING.md)

<h2 id="contribute">Code of conduct</h2>

See in the [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)

<h2 id="license">License</h2>

Distributed under the Apache License 2.0 License.
See [Apache License 2.0 License](https://www.apache.org/licenses/LICENSE-2.0) for more information.
