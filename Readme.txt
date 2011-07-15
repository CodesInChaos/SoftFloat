# SoftFloat floating point library
Certain network architectures require deterministic math. Since the normal `Single`/`Double` classes in .net can beave differently on different hardware they can't be used. Thus I decided to write a software implementation of 32 bit floating points.

The `SoftFloat` struct contains a 32 bit integer whose value conforms to the `binary32` datatype from IEEE 754. This means reinterpret casts to `Single`/`float` are possible. SubNorms, infinities and `NaN`s are supported.

The results for the basic operations should be within one step of the correct result(i.e. there may be small rounding errors). The complex operations may have larger errors.

Performance is an important goal. So if you see a way to make it faster, please contact me.

While I'm using a permissive license, I'd still be happy if you contributed back any improvements you make.