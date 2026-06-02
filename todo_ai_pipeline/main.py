"""
Entry point — watches TODO.md for create/update events and triggers the Orchestrator.

Usage:
    python main.py

Requires:
    ANTHROPIC_API_KEY environment variable set.
"""

import sys
import time
import os
from pathlib import Path

from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler, FileCreatedEvent, FileModifiedEvent

from orchestrator import Orchestrator

WATCH_FILE = Path(__file__).parent / "TODO.md"
DEBOUNCE_SECONDS = 2.0  # avoid double-trigger on rapid saves


class TodoFileHandler(FileSystemEventHandler):
    def __init__(self, orchestrator: Orchestrator):
        self.orchestrator = orchestrator
        self._last_trigger = 0.0

    def _should_handle(self, event) -> bool:
        if not isinstance(event, (FileCreatedEvent, FileModifiedEvent)):
            return False
        if Path(event.src_path).resolve() != WATCH_FILE.resolve():
            return False
        # Debounce
        now = time.time()
        if now - self._last_trigger < DEBOUNCE_SECONDS:
            return False
        self._last_trigger = now
        return True

    def on_created(self, event):
        if self._should_handle(event):
            print(f"\n[HOOK] TODO.md criado — iniciando pipeline...")
            self._run()

    def on_modified(self, event):
        if self._should_handle(event):
            print(f"\n[HOOK] TODO.md atualizado — iniciando pipeline...")
            self._run()

    def _run(self):
        try:
            self.orchestrator.process()
        except Exception as e:
            print(f"[ERROR] Pipeline falhou: {e}", file=sys.stderr)
            raise


def main():
    api_key = os.environ.get("ANTHROPIC_API_KEY")
    if not api_key:
        print("[ERROR] ANTHROPIC_API_KEY não está definida.", file=sys.stderr)
        sys.exit(1)

    print("=" * 60)
    print("  TODO AI Pipeline — File Watcher")
    print("=" * 60)
    print(f"  Watching: {WATCH_FILE}")
    print(f"  Agents: Researcher | Writer | Verifier | Reporter")
    print(f"  Memory: agents keep last 100 jobs each")
    print("=" * 60)
    print("  Aguardando alterações em TODO.md... (Ctrl+C para parar)\n")

    orchestrator = Orchestrator()
    handler = TodoFileHandler(orchestrator)

    observer = Observer()
    observer.schedule(handler, path=str(WATCH_FILE.parent), recursive=False)
    observer.start()

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("\n[INFO] Parando watcher...")
        observer.stop()
    observer.join()
    print("[INFO] Pipeline encerrado.")


if __name__ == "__main__":
    main()
