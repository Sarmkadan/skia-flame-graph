#!/usr/bin/env python3
"""
Utility script for the SkiaFlameGraph repository.

- Finds the repository root (the directory containing a .git folder).
- Executes `dotnet test` from the repository root.
- Returns the exit code from the test runner.

This script is intended to be invoked directly:
    python3 /home/redrocket/task-factory/aider_buildcmd.py
"""

import pathlib
import subprocess
import sys
from typing import Optional


def find_repo_root(start_path: pathlib.Path) -> pathlib.Path:
    """
    Walks up the directory tree from ``start_path`` until a directory containing a
    ``.git`` folder is found. Returns that directory as the repository root.

    Raises:
        FileNotFoundError: If no ``.git`` folder is found up to the filesystem root.
    """
    current = start_path.resolve()
    for _ in range(100):  # safeguard against infinite loops
        if (current / ".git").is_dir():
            return current
        if current.parent == current:
            break
        current = current.parent
    raise FileNotFoundError("Could not locate repository root (missing .git folder).")


def run_dotnet_test(repo_root: pathlib.Path) -> int:
    """
    Executes ``dotnet test`` in the given repository root.

    Returns:
        The exit code from the ``dotnet test`` process.
    """
    result = subprocess.run(
        ["dotnet", "test"],
        cwd=repo_root,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
    )
    # Echo the test output to the console for visibility
    print(result.stdout)
    return result.returncode


def main() -> int:
    """
    Entry point for the script.

    Returns:
        Exit code: 0 on success, non‑zero on failure.
    """
    try:
        # Use the directory containing this script as the starting point for locating the repo root.
        script_dir = pathlib.Path(__file__).parent
        repo_root = find_repo_root(script_dir)
    except FileNotFoundError as exc:
        print(f"Error: {exc}", file=sys.stderr)
        return 1

    return run_dotnet_test(repo_root)


if __name__ == "__main__":
    sys.exit(main())
