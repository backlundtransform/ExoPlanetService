const significantRound = (a: number) => {
    const numberofdigits =
        Math.max(Math.floor(Math.log10(Math.abs(Math.round(a)))), 0) + 1

    if (numberofdigits >= 3) {
        return Math.round(a)
    }

    if (numberofdigits === 2) {
        return Math.round(10 * a) / 10
    }

    if (Math.round(a) === 0) {
        return Math.round(1000 * a) / 1000
    }

    if (numberofdigits === 1) {
        return Math.round(100 * a) / 100
    }

    return a
}

export default significantRound