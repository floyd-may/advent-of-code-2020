function main(input) {
  const lines = input.split('\n').map((i ,k)=> {
    if (i.startsWith("mask = ")) {
      const mask = i.substr("mask = ".length);
      const b = BigInt("0b" + mask.replace(/[01]/g, "0").replace(/X/g, "1"));
      const a = BigInt("0b" + mask.replace(/X/g, "0"));
      return {c: "mask", a, b, };
    }
    const m = /mem\[(\d+)\] = (\d+)/.exec(i);
    if  (!m) throw "" +k + "@ " + i;
    return {c: "mem", a: BigInt(m[1]), b: BigInt(m[2]),};  
  });

  const mem = {};
  let mask = 0n, payload = 0n;
  for (let i of lines) {
    switch (i.c) {
      case "mask": mask = i.b; payload = i.a; break;
      case "mem": {
        if (typeof i.a != typeof mask) throw "" + (typeof i.a) + (typeof mask);
        const base = (i.a & ~mask) | payload;
        const v = i.b;
        gen_mask(mask, a => mem[base | a] = v);
        break;
      }
    }
  }
  console.log(Object.values(mem).reduce((a,i) => a + i, 0n));
}

function gen_mask(m, fn) {
  let n = 0;
  let ii = 1n;
  for (let j = m; j != 0; j &= j - 1n) { n++; ii *= 2n; }
  for (let i = 0n; i < ii; i++) {
    let k = 1n;
    let a = 0n;
    for (let j = m; j != 0; j &= j - 1n) {
      if ((i & k) != 0n)
        a |= j ^ (j & (j - 1n));
      k *= 2n;
    }
    fn(a);
  } 
}

const fs = require('fs');

const fullInput = fs.readFileSync('data/Day14.txt', 'utf8');

main(fullInput);
