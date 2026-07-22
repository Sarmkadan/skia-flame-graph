#!/usr/bin/env python3
"""
Simple build helper for the SkiaFlameGraph repository.

Running this script will invoke `dotnet test` on the repository,
printing the test results to stdout. It is intended to be used
as a quick way to verify that the project builds and all unit
tests pass.

The script searches upward from its own location for a `.git`
directory to locate the repository root. If the `.git` folder
cannot be found, it assumes the current directory is the root.
"""

import subprocess
import sys
import pathlib


def find_repo_root(start_path: pathlib.Path) -> pathlib.Path:
    """
    Walk up the directory tree until a `.git` folder is found.
    If none is found, return the original start_path.
    """
    current = start_path.resolve()
    while current != current.parent:
        if (current / ".git").is_dir():
            return current
        current = current.parent
    return start_path.resolve()


def run_dotnet_test(repo_root: pathlib.Path) -> int:
    """
    Execute `dotnet test` in the given repository root.
    Returns the process exit code.
    """
    try:
        result = subprocess.run(
            ["dotnet", "test", "--no-build", "--verbosity", "minimal"],
            cwd=repo_root,
            check=False,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
        )
        # Print the combined output so the caller sees the test results.
        print(result.stdout)
        return result.returncode
    except FileNotFoundError:
        print(
            "Error: 'dotnet' executable not found. Please install the .NET SDK.",
            file=sys.stderr,
        )
        return 1


def main() -> int:
    # Determine the repository root relative to this script.
    script_dir = pathlib.Path(__file__).parent
    repo_root = find_repo_root(script_dir)

    # Run the tests.
    return run_dotnet_test(repo_root)


if __name__ == "__main__":
    sys.exit(main())
