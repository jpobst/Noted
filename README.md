# Noted

Noted is a `dotnet` tool that can be used to generate automatic release 
notes from GitHub PR's.


## Usage

Install from NuGet with:

```
dotnet tool install -g noted
```

Run from `dotnet` command line:
```
noted
  --repository-owner jpobst 
  --repository noted 
  --output-directory "C:\code\noted.wiki"
  --repository-name Noted 
  --token OPTIONAL_GITHUB_PAT
```

You will need to have a page called `Release-Notes.md` in your GitHub wiki that contains
this text somewhere in it:

```
<!-- Begin: Release Notes Links -->
<!-- End: Release Notes Links -->
```

Links to individual release note pages will be added between these comments.

## How it Works

### Release Notes Generation

In order to be automatically added to release notes, a pull request must be:

- Merged
- Contain the `release-notes` label
- Be assigned to an open milestone

Pull requests meeting this criteria will be automatically added to the draft release 
notes in the `Issues fixed` section with a default entry based on the PR title:

```
### Issues fixed

#### Binding projects

- [Java.Interop GitHub PR 645](https://github.com/xamarin/java.interop/pull/645):  
  [XAT.Bytecode] Kotlin internal interfaces need to be set to package-private
```

#### Customizing the release note

Release notes can be customized to provide more details by adding a comment to the PR that
starts with a line mentioning "release note" and contains a markdown fence block with the 
desired message:

~~~
Release note:

```
Fixed an issue where Kotlin internal interfaces were being ignored, causing any 
public classes that implemented them to not be bound.
```
~~~

This will result in:

```
### Issues fixed

#### Binding projects

- [Java.Interop GitHub PR 645](https://github.com/xamarin/java.interop/pull/645):  
  Fixed an issue where Kotlin internal interfaces were being ignored, causing any 
  public classes that implemented them to not be bound.
```

#### Customizing the minor and major headers

By default notes go into the categories `Issues fixed` -> `Binding projects`.

This can be customized by using a `### Major Heading` and `#### Minor Heading`.
(Minor headers are only used when the major header is `Issues fixed`.)

For example:
~~~
Release note:

```
### Build perfomance

- Building a binding project that had a reference to another binding project 
  would do a bit of redundant work for assemblies referenced by both projects
```
~~~

Will result in:

```
### Build perfomance

- [Java.Interop GitHub PR 611](https://github.com/xamarin/java.interop/pull/611):  
  Building a binding project that had a reference to another binding project 
  would do a bit of redundant work for assemblies referenced by both projects
```