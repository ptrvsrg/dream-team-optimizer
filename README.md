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

**The Dream Team Optimizer** project is designed to optimally form development teams based on their preferences gathered
during the hackathon. Each developer (Juniors or TeamLeads) makes a list of desirable colleagues with whom he would like
to work in a team. Based on this data, the project calculates the satisfaction index for each participant, and then
calculates the harmony of the team distribution. The main goal of the project is to maximize the harmony of team
formation in order to ensure the greatest satisfaction of the participants. This tool can be useful for HR professionals
to optimize the process of creating dream teams.

<h2>Strategies</h2>

<h3>Gale–Shapley algorithm</h3>

The strategy is based on the [Gale-Shapley algorithm](https://en.wikipedia.org/wiki/Gale%E2%80%93Shapley_algorithm)
(also known as the deferred acceptance algorithm, the offer and rejection algorithm, or the Boston Pool algorithm). This
algorithm solves the problem of stable matching, which consists in pairing an equal number of participants of two types
using the preferences of each participant.

<h3>Strategy based on bipartite graph</h3>

In order to form teams, a complete weighted bipartite graph is built, such that there are juniors in one share, team
leaders in the other, and the edges have a weight equal to the sum of the vertex satisfaction indices. Next, the edges
are sorted by weight and teams are formed sequentially from the edge with the highest weight (after creating the team,
the corresponding vertices and edges are removed from the rating)

<h3>Weighted preference strategy</h3>

It is a modification of Gale–Shapley algorithm. By introducing weights into preferences, it is possible to balance on 
changing priorities in order to fairly take into account the interests of both sides. For example, if one side prefers 
instinctively, its preferences may carry more weight.

<h3>Comparison</h3>

<details>
  <summary>Machine parameters</summary>

+ **CPU:** AMD Ryzen 5 3500U with Radeon Vega Mobile Gfx
+ **RAM:** DDR4 8GB 3200 MHz * 2

</details>

<table>
    <tr>
        <th>Strategy</th>
        <th>CPU, %</th>
        <th>RAM, %</th>
        <th>Time, s</th>
        <th>Harmonicity</th>
    </tr>
    <tr>
        <td>Gale–Shapley algorithm</td>
        <td>209.667</td>
        <td>0.367</td>
        <td>91</td>
        <td>13.907</td>
    </tr>
    <tr>
        <td>Weighted preference strategy</td>
        <td>317</td>
        <td>0.367</td>
        <td>120</td>
        <td>14.157</td>
    </tr>
    <tr>
        <td>Strategy based on bipartite graph</td>
        <td>369.583</td>
        <td>0.375</td>
        <td>129</td>
        <td>14.335</td>
    </tr>
</table>

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

<h4>Locally</h4>

```bash
make build.console
HACKATHON_CONFIG_PATH=./src/DreamTeamOptimizer.ConsoleApp/appsettings.json \
  dotnet ./out/DreamTeamOptimizer.ConsoleApp.dll
```

<h4>Docker</h4>

```bash
make build-image.console
docker run \
  -v "./src/DreamTeamOptimizer.ConsoleApp/appsettings.json:/app/appsettings.json" \
  -v "./src/DreamTeamOptimizer.ConsoleApp/example:/app/example" ptrvsrg/dream-team-optimizer-console
```

<h2 id="contribute">Contribute</h2>

See in the [CONTRIBUTING.md](CONTRIBUTING.md)

<h2 id="contribute">Code of conduct</h2>

See in the [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)

<h2 id="license">License</h2>

Distributed under the Apache License 2.0 License.
See [Apache License 2.0 License](https://www.apache.org/licenses/LICENSE-2.0) for more information.
