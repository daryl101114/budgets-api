"""
Build script — assembles all module files into the importable Postman collection.

Usage:
    python postman/build.py

Output:
    Budget API.postman_collection.json  (at project root)
"""

import json
import os
import glob

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR   = os.path.dirname(SCRIPT_DIR)
OUTPUT     = os.path.join(ROOT_DIR, "Budget API.postman_collection.json")

def main():
    # Load meta (info + variables)
    meta_path = os.path.join(SCRIPT_DIR, "_meta.json")
    with open(meta_path, encoding="utf-8") as f:
        meta = json.load(f)

    # Load module files in numeric order (01_*, 02_*, ...)
    module_files = sorted(glob.glob(os.path.join(SCRIPT_DIR, "[0-9][0-9]_*.json")))

    folders = []
    for path in module_files:
        with open(path, encoding="utf-8") as f:
            folders.append(json.load(f))
        print(f"  Loaded: {os.path.basename(path)}  ({len(folders[-1].get('item', []))} requests)")

    collection = {
        "info":     meta["info"],
        "variable": meta["variable"],
        "item":     folders,
    }

    with open(OUTPUT, "w", encoding="utf-8") as f:
        json.dump(collection, f, indent=2)

    total = sum(len(folder.get("item", [])) for folder in folders)
    print(f"\nBuilt: {OUTPUT}")
    print(f"  {len(folders)} folders | {total} requests")

if __name__ == "__main__":
    main()
