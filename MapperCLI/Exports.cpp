#include "Exports.h"
#include "skCrypt.h"

void Map(BYTE* data, int pid) {
    HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, TRUE, pid);

    if (hProc == NULL) {
        return;
    }

    Inject(hProc, data);

    return;
}